using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SetUp : MonoBehaviour
{

    // 定数
    public const string TOGGLE_STAR_NAME = "ToggleStar_";  //!< シーン上の星の名称

    // 外部変数
    public PlayerEnhanceType m_StarType;
    public int starIndex = 1;    //!< 星インデックス
    public Enhance enhanceObject;
    public List<ToggleStar> m_StarToggleList = new List<ToggleStar>(); //!< 星のトグルコンポーネントリスト

    void Start() {
        // 星がなければ取ってくる
        if (m_StarToggleList.Count <= 0) {
            foreach (Transform star in this.transform) {
                // コンポーネントを追加してデータを設置する
                star.gameObject.AddComponent<ToggleStar>().SetData(OnClickStar);
                // 追加したコンポーネントを保持
                m_StarToggleList.Add(star.gameObject.GetComponent<ToggleStar>());
            }
        }

        // 初期値をセット
        SetToggle(5, starIndex);
    }

    /// <summary>
    /// 星をクリックした場合
    /// </summary>
    private ToggleStar m_SelectedStar = null;
    private bool changeSelectStar = false;
    private bool onceSelect = false;
    public bool OnClickStar(ToggleStar obj) {
        if (changeSelectStar) {
            if (enhanceObject.useEnhancePoint <= 0 && m_SelectedStar.name != obj.name && m_SelectedStar.GetMyId() <= obj.GetMyId()) {
                return false;
            }
            onceSelect = true;
        }

        // 設定された星に合わせる
        starIndex = obj.GetMyId();

        // 前回ポイントをここで保持
        int beforePoint = 1;
        if (m_SelectedStar != null) {
            beforePoint = LastIndexIsOnStar().GetMyId();
        }

        if (m_SelectedStar != null)
            m_SelectedStar = LastIndexIsOnStar();

        int shortage = enhanceObject.useEnhancePoint - (starIndex - beforePoint);
        if (shortage < 0) {
            starIndex += shortage;
            obj = m_StarToggleList.Single(toggle => toggle.name == TOGGLE_STAR_NAME + (enhanceObject.useEnhancePoint + beforePoint));
            changeSelectStar = true;
        }

        // 強化ポイントに反映
        enhanceObject.SetEnhancePoint(beforePoint, starIndex);
        // 今回の状態を保持
        m_SelectedStar = obj;
        // 星移動
        if (shortage < 0) enhanceObject.IsEnhance = false;
        SetToggle(5, starIndex);

        // 凄く汚いけど処理のタイミング的にここで切り替えないとisOnが取れない
        if (shortage < 0) {
            enhanceObject.unableIsOffStars();
        }
        else if (!enhanceObject.IsEnhance) {
            enhanceObject.AllEnableStars();
            enhanceObject.IsEnhance = true;
        }
        CommonSys.GetSystem<TitleWindow>().SoundButton(Data.d_SoundEffect[SoundEffectType.SETUP_BUTTON]);
        return true;
    }

    /// <summary>
    /// 押された星以下は全てONにする
    /// </summary>
    /// <param name="index">星のインデックス</param>
    /// <param name="setId">選択された星</param>
    private void SetToggle(int index, int setId) {
        // 0であれば返す
        if (index <= 0) return;
        ToggleStar star = GetStarInToggleList(index);
        // 一旦関数を落とす
        star.GetToggle().onValueChanged.RemoveAllListeners();
        // 押された星以下は全てONにする
        star.IsOn = setId >= index;
        star.GetToggle().isOn = setId >= index;
        // 関数をセット
        star.SetToggleFunc();

        // 次に進む
        SetToggle(index - 1, setId);
    }

    /// <summary>
    /// トグルリストから任意のオブジェクトを取得する
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private ToggleStar GetStarInToggleList(int index) {
        return m_StarToggleList.Single(obj => obj.gameObject.name == TOGGLE_STAR_NAME + index);
    }

    public void AllenableStars() {
        m_StarToggleList.ForEach(toggle => toggle.GetToggle().enabled = true);
    }

    public void UnableIsOffStars() {
        foreach (ToggleStar star in m_StarToggleList) {
            if (star.GetToggle().isOn == false) {
                star.GetToggle().enabled = false;
            }
        }
    }

    private ToggleStar LastIndexIsOnStar() {
        ToggleStar i = m_StarToggleList.FindLast(cnt => cnt.IsOn == true);
        return i == null ? m_StarToggleList[0] : i;
    }
}
