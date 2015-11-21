using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Enhance : MonoBehaviour {

    // 定数
    const int MAX_ENHANCE_POINT = 10;               //!< 強化ポイントの最大値

    // 外部プロパティ
    public int useEnhancePoint {
        get { return _useEnhancePoint; }
    }

    private bool isEnhance = true;
    public bool IsEnhance {
        get { return isEnhance; }
        set { isEnhance = value; }
    }

    // 外部変数
    public Text enhanceDispText;    //!< 強化ポイント表示テキスト
    public List<GameObject> riseButtonList = new List<GameObject>();    //!< 上昇ボタンリスト
    public List<SetUp> starSetUpList = new List<SetUp>();               //!< 星のセットアップリスト

    // 内部変数
    private int _useEnhancePoint = MAX_ENHANCE_POINT;   //!< 使用した強化ポイント

    void Start() {
        DispEnhancePoint();
    }

    /// <summary>
    /// 強化ポイントをセット
    /// 値をそのまま切り替える
    /// </summary>
    /// <param name="value">変更値</param>
    public void SetEnhancePoint(int value) {
        _useEnhancePoint -= value;
        DispEnhancePoint();
    }

    /// <summary>
    /// 強化ポイントをセット
    /// 値を計算して切り替える
    /// </summary>
    /// <param name="val_from">前回値</param>
    /// <param name="val_to">変更値</param>
    public void SetEnhancePoint(int val_from, int val_to) {
        val_to -= val_from;
        _useEnhancePoint -= val_to;
        DispEnhancePoint();
    }

    /// <summary>
    /// 強化ポイントを表示
    /// </summary>
    private void DispEnhancePoint() {
        enhanceDispText.text = _useEnhancePoint.ToString();
        VisibleRiseButtons();
    }

    public void AllEnableStars() {
        foreach (SetUp set in starSetUpList) {
            set.AllenableStars();
        }
    }

    public void unableIsOffStars() {
        foreach (SetUp set in starSetUpList) {
            set.UnableIsOffStars();
        }
    }

    /// <summary>
    /// 強化ポイントが0以下になったらボタンを消す
    /// </summary>
    private void VisibleRiseButtons() {
        // 強化値をセットする
        SetPlayerParam();
    }

    /// <summary>
    /// 強化値をセット
    /// </summary>
    private void SetPlayerParam() {
        YkSys.playerParam.playerEnhanceLifePt           = starSetUpList.Find(type => type.m_StarType == PlayerEnhanceType.LIFE        ).m_StarToggleList.Count(on => on.GetToggle().isOn == true);
        YkSys.playerParam.playerEnhanceSpeedPt          = starSetUpList.Find(type => type.m_StarType == PlayerEnhanceType.SPEED       ).m_StarToggleList.Count(on => on.GetToggle().isOn == true);
        YkSys.playerParam.playerEnhanceShotSpeedPt      = starSetUpList.Find(type => type.m_StarType == PlayerEnhanceType.SHOT_SPEED  ).m_StarToggleList.Count(on => on.GetToggle().isOn == true);
        YkSys.playerParam.playerEnhanceChargeSpeedPt    = starSetUpList.Find(type => type.m_StarType == PlayerEnhanceType.CHARGE_SPEED).m_StarToggleList.Count(on => on.GetToggle().isOn == true);
    }
}
