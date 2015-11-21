using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class YkSys : CommonSys {

    // 定数
    public const int STAGE_NUM      = 2;  //!< ステージ数
    public const int CONTINUE_NUM   = 3;  //!< コンティニュー回数
    public const string SCENARIO_BEGIN_ANIMATION_NAME = "Start"; //!< シナリオ開始アニメーション名

    // 外部変数
    public bool boss = false;   //!< ボス出現フラグ
    public bool win = false;    //!< ボス勝利フラグ
    public bool clear = false;  //!< クリアフラグ
    public bool debug = false;

    // 共有変数
    protected static CommonSoundForCri m_BGM; //!< BGM
    protected ScenarioPatternType scenarioFlag = ScenarioPatternType.NONE;  //!< 現在のシナリオフラグ

    public float TotalScore {
        get { return totalScore; }
        protected set { totalScore = value; }
    }

    /// <summary>
    /// Awake
    /// </summary>
    public override void Awake() {
        base.Awake();

        // ステージ状態をセット
        nowStage += 1;
        if (Application.loadedLevelName == Data.d_StageNameList[1]) nowStage = Stage.title;
        if (debug) nowStage = Stage.stage1;

        // シナリオデータを読み込む(正直、都度読み込む方が都合が良いけれども今回読み込む量はそんなに多くないはずなのでスタックしてしまう)
        if (scenarioData.Count <= 0) {
            StartCoroutine(ReadScenarioData());
        }

        if (stageUIDic.Count <= 0) {
            SetStageUISprites();
        }

        // 弾丸データを読み込む
        BulletBase.CreateEnemyBulletGroupData();
        BulletBase.CreateEnemyBulletRadData();
        DirectionData.CreateBezierDirectionData();

        // 通常敵AIデータを読み込む
        EnemyFactory.CreateNormalEnemyAiData();

        // 背景読み込み
        StageBackground.CreateBackgroundImageData();
        StageBackground.CreateBackgroundImageSettingData();
    }

    /// <summary>
    /// 画面ポーズ
    /// </summary>
    public static bool Pose {
        protected set { enable = value;}
        get { return enable; }
    }

    /// <summary>
    /// シナリオデータを読み込み
    /// </summary>
    /// <returns></returns>
    IEnumerator ReadScenarioData() {
        int parse = 0;
        TextAsset[] textData = Resources.LoadAll<TextAsset>("Scenario/");
        // 取ってきたテキストを必要に応じて分類する
        foreach (TextAsset txt in textData) {
            // 中身をバラして一行毎に読み込む
            List<string> text = txt.text.Split(new char[]{'\n', '\r'}).ToList<string>();
            CommonUtil.Unset(ref text);

            // 一番最初のコマンドを読む
            foreach (string val in text) {
                if (val.Substring(0, 1) == "@") {
                    string[] optCom = val.Split(new char[] { ' ' });
                    // 数値以外は通さないように
                    if (int.TryParse(optCom[2], out parse)) {
                        scenarioData.Add(new ScenarioData((ScenarioPatternType)parse, GetStringToStageEnum(optCom[1]), txt, false));
                    }
                    break;
                }
            }
        }
        yield return null;
    }

    /// <summary>
    /// ステージ名をEnum形式に変更
    /// </summary>
    /// <param name="stage">ステージ名</param>
    /// <returns>Stage(Enum)</returns>
    public Stage GetStringToStageEnum(string stage) {
        Stage stageEnum = Stage.none;
        foreach (var name in Data.d_StageNameList.Select((value, index) => new { index, value})) {
            if (name.value == stage) {
                stageEnum = (Stage)name.index;
                break;
            }
        }
        return stageEnum;
    }

    /// <summary>
    /// 適切なシナリオデータを取得する
    /// </summary>
    /// <returns>該当シナリオデータ</returns>
    public ScenarioData GetScenarioData(ScenarioPatternType type) {
        return scenarioData.Single(sc => sc.StageState == nowStage && sc.ScenarioType == type);
    }

    void SetStageUISprites() {
        Sprite[] images = Resources.LoadAll<Sprite>("Sprites/UI/ui_stage");
        foreach (Sprite sp in images) {
            stageUIDic.Add(sp.name, sp);
        }
    }

    public Sprite GetStageUISprites(string name) {
        Sprite ui = null;
        if (stageUIDic.ContainsKey(name)) {
            ui = stageUIDic[name];
        }
        return ui;
    }

    /*************************************************************************
     * 全てのシーンで共有する変数
     *************************************************************************/
    public static Player.PlayerEnhanceParam playerParam = new Player.PlayerEnhanceParam(5);    // プレイヤー強化パラメータ

    public static Stage nowStage    = Stage.none;           // 現在のステージを指定
    public static bool scenarioSkip = false;                // シナリオスキップフラグ
    public static bool battleSkip = false;                  // バトルスキップフラグ
    public static bool mutekiFlag = false;                  // 無敵フラグ
    
    /*************************************************************************
     * 必要なシーンで共有する変数
     *************************************************************************/
    protected static bool enable = true;    // 画面全体の挙動フラグ
    protected static float totalScore;      // 獲得した総スコア
    protected static List<ScenarioData> scenarioData = new List<ScenarioData>();    //!< シナリオデータ
    protected static Dictionary<string, Sprite> stageUIDic = new Dictionary<string, Sprite>(); //!< ステージUI一覧
}
