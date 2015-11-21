using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BattleDebug : YkSys {

    [SerializeField]
    GameObject playerPrefab;    //!< プレイヤープレハブ

    // 定数
    public const string BUTTLE_DEBUG_SCENE_NAME = "battleDebug";

    // 外部変数
    public EnemyFactory factory;
    public GameObject restartButton;    //!< リスタートボタン用イメージ
    public GameObject player;           //!< プレイヤーオブジェクト
    public GameObject resultPanel;      //!< リザルトオブジェクト
    public GameObject help;             //!< ヘルプオブジェクト
    public Slider bossHpSlider;         //!< ボス体力用スライダー
    public Slider hpSlider;             //!< 体力表示用スライダー
    public Slider bulletEnergyGauge;    //!< 弾エネルギーゲージ
    public Slider PlayerOptGauge;       //!< プレイヤーオプションゲージ
    public Text damagePerText;          //!< 被害率表示用テキスト
    public Text scoreText;              //!< スコアを表示
    public bool stopEscKey = false;     //!< Escキーを止めるフラグ
    public CommonSoundForCri bgmSound;
    public string cueName = "";

    // 内部変数
    private float playerScore;          //!< スコア
    public bool debugStart = false;
    private string dirRoot;
    private bool call = false;

    public override void Awake() {
        base.Awake();

        // ステージ１で固定する
        YkSys.nowStage = Stage.stage1;

        // 音源再生
        m_BGM = bgmSound;
        m_BGM.CueSheetNameBGM = "BGM";

        restartButton.SetActive(false);
    }

    void Start() {
        dirRoot = Directory.GetCurrentDirectory() + @"\Battle";
        if (!Directory.Exists(dirRoot)) {
            Directory.CreateDirectory(dirRoot);
        }
        TextAsset[] scenarioAsset = Resources.LoadAll<TextAsset>("EnemyFormationData");
        foreach (var data in scenarioAsset) {
            if (!File.Exists(dirRoot + @"\" + data.name + ".csv")) {
                using (StreamWriter sw = new StreamWriter(dirRoot + @"\" + data.name + ".csv", true, Encoding.UTF8)) {
                    sw.Write(data.text);
                }
            }
        }
        TextAsset[] bulletAsset = Resources.LoadAll<TextAsset>("EnemyBulletData");
        foreach (var data in bulletAsset) {
            if (!File.Exists(dirRoot + @"\" + data.name + ".csv")) {
                using (StreamWriter sw = new StreamWriter(dirRoot + @"\" + data.name + ".csv", true, Encoding.UTF8)) {
                    sw.Write(data.text);
                }
            }
        }
        TextAsset directionAsset = Resources.Load<TextAsset>("DirectionData/BezierData");
        if (!File.Exists(dirRoot + @"\BezierData.csv")) {
            using (StreamWriter sw = new StreamWriter(dirRoot + @"\BezierData.csv", true, Encoding.UTF8)) {
                sw.Write(directionAsset.text);
            }
        }
        YkSys.Pose = !YkSys.Pose;
        // 音量を設定
        OPTION.SetVolume(OPTION.BGMVolume, OPTION.Sound.BGM, m_BGM);
        OPTION.SetVolume(OPTION.SEVolume, OPTION.Sound.SE);
    }

    void Update() {
        if (debugStart) {
            // ESCキーで全体を止める
            if (Input.GetKeyDown(KeyCode.Escape) && !stopEscKey) {
                Pose = !Pose;
                if (!Pose) {
                    m_BGM.Pause();
                    help.SetActive(true);
                }
                else {
                    m_BGM.Pause();
                    help.SetActive(false);
                }
            }

            if (stopEscKey) {
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
                    }
                }
            }


            // クリアしたらリザルトを表示
            if (clear) {
                resultPanel.GetComponent<Result>().SetData(player.GetComponent<DebugPlayer>().Score);
                resultPanel.SetActive(true);
                Pose = !Pose;
                clear = false;
            }
        }
    }

    void OnGUI() {
        if (!debugStart) {
            if (GUI.Button(new Rect(500, 500, 100, 50), "BattleStart")) {
                if (!call) {
                    CallEnemy();
                    call = true;
                }
                if (player == null) {
                    player = CommonUtil.PrefabInstance(playerPrefab);
                }
                YkSys.Pose = !YkSys.Pose;
                debugStart = true;
                m_BGM.Play(cueName);
            }

            if (GUI.Button(new Rect(100, 200, 200, 80), "再読み込み")) {
                call = false;
                factory.Reset();
            }

            if (GUI.Button(new Rect(1100, 100, 100, 50), Data.d_StageBgmTable[Stage.stage1])) {
                cueName = Data.d_StageBgmTable[Stage.stage1];
            }
            if (GUI.Button(new Rect(1100, 190, 100, 50), Data.d_StageBgmTable[Stage.stage2])) {
                cueName = Data.d_StageBgmTable[Stage.stage2];
            }
            if (GUI.Button(new Rect(1100, 280, 100, 50), Data.d_StageBgmTable[Stage.stage3])) {
                cueName = Data.d_StageBgmTable[Stage.stage3];
            }
            if (GUI.Button(new Rect(1100, 370, 100, 50), Data.d_StageBgmTable[Stage.stage4])) {
                cueName = Data.d_StageBgmTable[Stage.stage4];
            }
            if (GUI.Button(new Rect(1100, 460, 100, 50), Data.d_StageBgmTable[Stage.stage5])) {
                cueName = Data.d_StageBgmTable[Stage.stage5];
            }
            if (GUI.Button(new Rect(1100, 550, 100, 50), Data.d_StageBgmTable[Stage.stage6])) {
                cueName = Data.d_StageBgmTable[Stage.stage6];
            }
        }
    }

    void CallEnemy() {
        string aiData = null, enemyData = null, bossData = null, bezData = null, bulletGD = null, bulletRD = null;
        foreach (string data in Directory.GetFiles(dirRoot)) {
            using (StreamReader sr = new StreamReader(data, Encoding.UTF8)) {
                if (data.Contains("BezierData")) {
                    bezData = sr.ReadToEnd();
                    DirectionData.CreateBezierDirectionData(bezData);
                }
                if (data.Contains("NormalEnemyAIData")) {
                    aiData = sr.ReadToEnd();
                    EnemyFactory.CreateNormalEnemyAiData(aiData);
                }
                if (data.Contains("enemy_normal_stage1")) {
                    enemyData = sr.ReadToEnd().Clone() as string;
                }
                if (data.Contains("enemy_boss")) {
                    bossData = sr.ReadToEnd().Clone() as string;
                }
                if (data.Contains("EnemyBulletGroupData")) {
                    bulletGD = sr.ReadToEnd();
                    EnemyBulletBase.CreateEnemyBulletGroupData(bulletGD);
                }
                if (data.Contains("EnemyBulletRadData")) {
                    bulletRD = sr.ReadToEnd();
                    EnemyBulletBase.CreateEnemyBulletRadData(bulletRD);
                }
            }
            if (enemyData != null && bossData != null && aiData != null && bezData != null) {
                factory.init(enemyData, bossData);
                factory.CreateInitObjects();
            }
        }
    }

    /// <summary>
    /// スコアを設定
    /// </summary>
    /// <param name="score">現在のスコア</param>
    public void SetScoreText(float score) {
        scoreText.text = string.Format("{0:0000000000000000}", TotalScore + score);
    }

    /// <summary>
    /// 合計スコアをセット
    /// </summary>
    /// <param name="score">獲得したスコア</param>
    public void SetTotalScore(float score) {
        if (score > 0) TotalScore += score;
    }

    /// <summary>
    /// リスタートボタン
    /// </summary>
    public void OnClickRestartButton() {
        player.GetComponent<DebugPlayer>().Restart();
    }
}
