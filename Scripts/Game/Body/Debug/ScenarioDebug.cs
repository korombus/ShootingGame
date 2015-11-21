using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScenarioDebug : MonoBehaviour {
    /*
    [SerializeField]
    Animator StartAnim;         //!< ステージ開始アニメーション
    public RuntimeAnimatorController cont;

    // 外部変数
    public string dirRoot;
    public GameObject scenarioObject;   //!< シナリオ再生用オブジェクト
    public CommonSoundForCri m_BGM;
    public bool Pose = false;
    public ScenarioPatternType scenarioFlag = ScenarioPatternType.NONE;  //!< 現在のシナリオフラグ
    public ScenarioPatternType scenarioState = ScenarioPatternType.START; //!< シナリオの状態
    public Stage nowStage;

    public bool debugStart = false;
    public List<string> fileNameList = new List<string>();
    public ErrorText error;

    public GameObject BackGroundObj;
    public GameObject Plane;

    // 内部変数
    private List<ScenarioDebugData> scData = new List<ScenarioDebugData>();
    private ReadScenario scenario;      //!< シナリオ読み
    private bool stopEscKey = false;    //!< Escキーを止めるフラグ
    private bool bgmFade = false;       //!< bgmフェードフラグ
    private float bgmVolume = 0f;            //!< bgmボリューム
    private bool boss = false;
    private bool win = false;
    private bool clear = false;
    private bool battleSkip = true;
    private bool openingDebug = false;

    void Start() {
        dirRoot = Directory.GetCurrentDirectory() + @"\Scenario";
        if (!Directory.Exists(dirRoot)) {
            Directory.CreateDirectory(dirRoot);
        }
        TextAsset[] scenarioAsset = Resources.LoadAll<TextAsset>("Scenario");
        foreach (var data in scenarioAsset) {
            if (!File.Exists(dirRoot + @"\" + data.name + ".txt")) {
                using (StreamWriter sw = new StreamWriter(dirRoot + @"\" + data.name + ".txt", true, Encoding.UTF8)) {
                    sw.Write(data.text);
                }
            }
        }
        GetAllScenarioData(dirRoot);

        TextAsset textData = Resources.Load<TextAsset>("Sounds/ComboBGM");
        List<string> text = textData.text.Split(new char[]{'\n', '\r'}).ToList<String>();
        CommonUtil.Unset(ref text);
        text.RemoveAll(rem => rem.Substring(0, 2) == "//");

        foreach (string data in text) {
            List<string> list = data.Split(new char[] { ',' }).ToList<string>();

            if (list[1] != "") master.Add(new SoundComboMaster(list[1], Resources.Load<AudioClip>(CommonSound.BGMPath + list[1]), float.Parse(list[3]), SoundComboType.TYPE_A));
            if (list[2] != "") master.Add(new SoundComboMaster(list[2], Resources.Load<AudioClip>(CommonSound.BGMPath + list[2]), 0, SoundComboType.TYPE_B));
        }
    }

    void GetAllScenarioData(string dir) {
        foreach (var data in Directory.GetFiles(dir)) {
            if (!fileNameList.Exists(file => file == data.Replace(dirRoot, "").Replace(".txt", "").Replace(".meta", ""))) {
                fileNameList.Add(data.Replace(dirRoot + @"\", "").Replace(".txt", ""));
            }
            int parse = 0;
            if (data.Contains(".meta")) {
                continue;
            }
            using (StreamReader sr = new StreamReader(data, Encoding.UTF8)) {
                string fullText = sr.ReadToEnd();
                // 中身をバラして一行毎に読み込む
                List<string> text = fullText.Split(new char[] { '\n', '\r' }).ToList<string>();
                text.RemoveAll(none => none == "\n" || none == "\r" || none == "");
                // 一番最初のコマンドを読む
                foreach (string val in text) {
                    if (val.Substring(0, 1) == "@") {
                        string[] optCom = val.Split(new char[] { ' ' });
                        // 数値以外は通さないように
                        if (int.TryParse(optCom[2], out parse)) {
                            scData.Add(new ScenarioDebugData((ScenarioPatternType)parse, GetStringToStageEnum(optCom[1]), fullText, false));
                        }
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// ステージ名をEnum形式に変更
    /// </summary>
    /// <param name="stage">ステージ名</param>
    /// <returns>Stage(Enum)</returns>
    private Stage GetStringToStageEnum(string stage) {
        Stage stageEnum = Stage.none;
        foreach (var name in Data.d_StageNameList.Select((value, index) => new { index, value })) {
            if (name.value == stage) {
                stageEnum = (Stage)name.index;
                break;
            }
        }
        return stageEnum;
    }


    void Update() {
        if (debugStart) {
            if (!scenarioObject.activeSelf || StartAnim.GetCurrentAnimatorStateInfo(0).IsName("End")) {

                if (stopEscKey) {
                    // マウスを押している間、読む速度上昇
                    if (Input.GetMouseButton(0)) {
                        scenario.OnClickDisplay();
                    }
                    // シナリオ読み出し
                    Pose = scenario.ReadScenarioText();
                    // ポーズが解けたらシナリオ用オブジェクト停止してゲームをスタート
                    if (Pose) {
                        stopEscKey = false;
                        scenarioObject.SetActive(false);
                        scenarioFlag = ScenarioPatternType.NONE;
                        // 勝利シナリオが終了したらクリアフラグを立てる
                        if (win) {
                            // クリアであれば音楽は流さない
                            m_BGM.Stop();
                            win = false;
                            clear = true;
                        }

                        // ボスシナリオを読み終わったらフラグを切る
                        if (boss) {
                            boss = false;
                            // バトルスキップの場合はここで勝利フラグを立てる
                            if (battleSkip) {
                                win = true;
                                scenarioState = ScenarioPatternType.WIN;
                            }
                        }
                        else {
                            if (openingDebug) {
                                clear = true;
                            }
                            else {
                                boss = true;
                                scenarioState = ScenarioPatternType.BOSS;
                            }
                        }
                    }
                }

                // ボスシナリオフラグ
                if (!stopEscKey && boss) {
                    CommandReadScenario();
                }

                // 勝利シナリオへ
                if (!stopEscKey && win) {
                    CommandReadScenario();
                }

                // デバッグフラグを折る
                if (clear) {
                    scenarioState = ScenarioPatternType.START;
                    m_BGM.Stop();
                   // CommonUtil.SearchObjectChild("BGM", m_BGM.gameObject.transform).GetComponent<CommonSound>().Stop();
                    StartAnim.runtimeAnimatorController = null;
                    boss = false;
                    win = false;
                    clear = false;
                    debugStart = false;
                    openingDebug = false;
                    scenarioObject.SetActive(false);
                    BackGroundObj.SetActive(true);
                    Plane.SetActive(true);
                }
            }

            // BGMフェード
           // scenario.FadeBGM();
           // scenario.CommandFadeBGM();

            // 主で流れているBGMをフェードさせる
            if (bgmFade) {
                FadeBGM();
            }
        }
    }

    /// <summary>
    /// シナリオ読み込み命令
    /// </summary>
    private void CommandReadScenario() {

        // シナリオデータをセット
        scenario = new ReadScenario(null, scenarioObject, m_BGM, GetScenarioData().Scenario);

        // 初期のアニメーションをセット
        if (StartAnim.runtimeAnimatorController == null) {
            StartAnim.runtimeAnimatorController = cont;
        }

        // シナリオを読む際に必要なフラグをセット
        scenarioObject.SetActive(true);
        Pose = false;
        stopEscKey = true;
        bgmFade = true;

        // 状態によって演出のフラグを立てる
        switch (scenarioState) {
            case ScenarioPatternType.BOSS:
            case ScenarioPatternType.WIN:
                StartAnim.SetBool("Boss", true);
                break;
        }
    }

    /// <summary>
    /// BGMをフェードさせる
    /// </summary>
    private void FadeBGM() {
        if (m_BGM.CheckPlayingAudio()) {
            if (bgmVolume <= 0) {
                bgmFade = false;
                bgmVolume = OPTION.BGMVolume;
            }
            else {
                bgmVolume -= 0.01f;
                m_BGM.ChangeVolume(bgmVolume < 0 ? 0 : bgmVolume);
            }
        }
        else {
            if (bgmVolume <= 0) {
                bgmFade = false;
                bgmVolume = OPTION.BGMVolume;
            }
            else {
                bgmVolume -= 0.01f;
            //    CommonUtil.SearchObjectChild("BGM", m_BGM.gameObject.transform).GetComponent<CommonSound>().ChangeVolume(bgmVolume < 0 ? 0 : bgmVolume);
            }
        }
    }

    /// <summary>
    /// 適切なシナリオデータを取得する
    /// </summary>
    /// <returns></returns>
    private ScenarioDebugData GetScenarioData() {
        return scData.Single(sc => sc.StageState == nowStage && sc.ScenarioType == scenarioState);
    }

    void OnGUI() {

        if (!debugStart && GUI.Button(new Rect(200, 200, 300, 100), "Refresh")) {
            scData.Clear();
            GetAllScenarioData(dirRoot);
        }

        if (!debugStart && ExistOpeningText() && GUI.Button(new Rect(720, 200, 300, 100), "Opening Debug")) {
            nowStage = Stage.opening;
            scenarioState = ScenarioPatternType.OPENING;
            CommandReadScenario();
            BackGroundObj.SetActive(false);
            Plane.SetActive(false);
            openingDebug = true;
            debugStart = true;
        }

        if (!debugStart && ExistStageText(Stage.stage1) && GUI.Button(new Rect(720, 300, 300, 100), "Stage1 Debug")) {
            nowStage = Stage.stage1;
            CommandReadScenario();
            debugStart = true;
        }

        if (!debugStart && ExistStageText(Stage.stage2) && GUI.Button(new Rect(720, 400, 300, 100), "Stage2 Debug")) {
            nowStage = Stage.stage2;
            CommandReadScenario();
            debugStart = true;
        }
    }

    private bool ExistStageText(Stage st) {
        return fileNameList.Exists(n => n == st + "_start" || n == st + "_boss" || n == st + "_win");
    }

    private bool ExistOpeningText() {
        return fileNameList.Exists(n => n == Stage.opening.ToString() + "_" + Stage.opening.ToString());
    }
     * */
}
