using UnityEngine;
using System.Collections;

public class Opening : YkSys {

    // 外部変数
    public GameObject selectObj;        //!< シナリオ選択オブジェクト
    public GameObject scenarioObj;      //!< シナリオオブジェクト
    public Animator scenarioObjAnim;    //!< シナリオオブジェクトのアニメータ
    public CommonSoundForCri bgmSound;

    // 内部変数
    private ReadScenario read;              //!< シナリオ読みクラス
    private bool scenarioEndFlag = false;   //!< シナリオ読み終わりフラグ
    private bool scenarioStart = false;     //!< シナリオ始まり

    public override void Awake() {
        base.Awake();
        if (debug) nowStage = Stage.opening;
        read = new ReadScenario(GetScenarioData(ScenarioPatternType.OPENING).Scenario, scenarioObj, bgmSound);

        // 音量を設定
        OPTION.SetVolume(OPTION.BGMVolume, OPTION.Sound.BGM, bgmSound);
        OPTION.SetVolume(OPTION.SEVolume, OPTION.Sound.SE);
    }
	
	void Update () {
        if (scenarioStart && scenarioObjAnim.GetCurrentAnimatorStateInfo(0).IsName("End")) {
            // マウスを押している間、読む速度上昇
            if (Input.GetKey(KeyCode.C) || Input.GetMouseButton(0)) {
                read.OnClickDisplay();
            }
            // シナリオ読み出し
            scenarioEndFlag = read.ReadScenarioText();

            // シナリオが読み終わったらステージへ
            if (scenarioEndFlag) {
                Application.LoadLevel("stage");
            }
        }
	}

    /// <summary>
    /// シナリオ開始ボタン
    /// </summary>
    public void OnClickScenarioStart() {
        selectObj.SetActive(false);
        scenarioObj.SetActive(true);
        scenarioStart = true;
    }

    /// <summary>
    /// シナリオスキップボタン
    /// </summary>
    public void OnClickScenarioSkip() {
        selectObj.SetActive(false);
        Application.LoadLevel("stage");
    }
}
