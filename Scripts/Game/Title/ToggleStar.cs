using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ToggleStar : MonoBehaviour {

    // プロパティ
    private bool _isOn;
    public bool IsOn {
        get { return _isOn; }
        set { _isOn = value; }
    }

    // 内部変数
    private Func<ToggleStar, bool> m_func;  //!< デリゲート関数

    public void SetData(Func<ToggleStar, bool> func) {
        m_func = func;
        // 自身を伏せる
        GetToggle().isOn = false;
        if (GetMyId() <= 1) GetToggle().isOn = true;
        // クリックされた時に反応するように自身を登録する
        GetToggle().onValueChanged.AddListener((value) => OnClickStar(value));

        // TODO:一旦機能を停止
        // GetToggle().enabled = false;
    }

    /// <summary>
    /// 自身のトグルコンポーネントを送る
    /// </summary>
    /// <returns></returns>
    public Toggle GetToggle() {
        return this.gameObject.GetComponent<Toggle>();
    }

    /// <summary>
    /// トグル関数を設置
    /// </summary>
    public void SetToggleFunc() {
        if (GetToggle().onValueChanged.GetPersistentEventCount() <= 0) {
            GetToggle().onValueChanged.AddListener((value) => OnClickStar(value));
        }
    }

    /// <summary>
    /// 星がクリックされたら自身を親に送る
    /// </summary>
    public void OnClickStar(bool state) {
        GetToggle().isOn = m_func(this);
    }

    /// <summary>
    /// 自身の数字を返す
    /// </summary>
    /// <returns></returns>
    public int GetMyId() {
        return int.Parse(gameObject.name.Replace(SetUp.TOGGLE_STAR_NAME, ""));
    }
}
