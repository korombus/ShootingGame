using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TopWindow : YkSys{

    [SerializeField]
    GameObject playerPrefab;    //!< プレイヤープレハブ

    [SerializeField]
    Animator StartAnim;         //!< ステージ開始アニメーション

    // 外部変数
    public GameObject restartButton;    //!< リスタートボタン用イメージ
    public GameObject player;           //!< プレイヤーオブジェクト
    public GameObject scenarioObject;   //!< シナリオ再生用オブジェクト
    public GameObject resultPanel;      //!< リザルトオブジェクト
    public GameObject help;             //!< ヘルプオブジェクト
    public Slider bossHpSlider;         //!< ボス体力用スライダー
    public Slider hpSlider;             //!< 体力表示用スライダー
    public Slider bulletEnergyGauge;    //!< 弾エネルギーゲージ
    public Slider PlayerOptGauge;       //!< プレイヤーオプションゲージ
    public Text damagePerText;          //!< 被害率表示用テキスト
    public Text scoreText;              //!< スコアを表示
    public Image stageName;             //!< ステージ名
    public CommonSoundForCri bgmSound;

    public ScenarioPatternType scenarioState = ScenarioPatternType.START; //!< シナリオの状態

    // 内部変数
    private ReadScenario scenario;      //!< シナリオ読み
    private bool stopEscKey = false;    //!< Escキーを止めるフラグ
    private float playerScore;          //!< スコア

    public override void Awake() {
        base.Awake();
        // プレイヤー生成
        if (nowStage != Stage.title) {
            player = CommonUtil.PrefabInstance(playerPrefab);
        }

        m_BGM = bgmSound;
        restartButton.SetActive(false);

        // ステージ名変更
        stageName.sprite = GetStageUISprites(nowStage.ToString());
    }

    void Start() {
        // 音量を設定
        OPTION.SetVolume(OPTION.BGMVolume, OPTION.Sound.BGM, m_BGM);
        OPTION.SetVolume(OPTION.SEVolume, OPTION.Sound.SE);

        // シナリオがあれば読み出す
        if (scenarioData.Count >= 1 && !scenarioSkip) {
            CommandReadScenario();
        }
        else {
            scenarioObject.SetActive(false);
        }

    }

    void Update() {
        if (!scenarioObject.activeSelf || StartAnim.GetCurrentAnimatorStateInfo(0).IsName("End")) {
            // ESCキーで全体を止める
            if (Input.GetKeyDown(KeyCode.Escape) && !stopEscKey) {
                Pose = !Pose;
                m_BGM.Pause();
                help.SetActive(!Pose);
            }

            if (stopEscKey) {
                // マウスを押している間、読む速度上昇
                if (Input.GetKey(KeyCode.C) || Input.GetMouseButton(0)) {
                    scenario.OnClickDisplay();
                }
                // シナリオ読み出し
                Pose = scenario.ReadScenarioText();

                // ポーズが解けたらシナリオ用オブジェクト停止してゲームをスタート
                if (Pose) {
                    stopEscKey = false;
                    scenarioObject.SetActive(false);
                    scenarioFlag = ScenarioPatternType.NONE;

                    // BGMを流す
                    m_BGM.SetCueSheetName(Data.d_CriCueSheetNameTable[SoundPatternCriAtom.BGM]);
                    m_BGM.Play(Data.d_StageBgmTable[nowStage]);

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
                }
            }

            // ボスシナリオフラグ
            if (!stopEscKey && boss && !scenarioSkip) {
                CommandReadScenario();
            }
            // シナリオがスキップされている場合はクリアフラグだけ立てる
            else if (boss && scenarioSkip) {
                clear = true;
            }

            // 勝利シナリオへ
            if (!stopEscKey && win) {
                CommandReadScenario();
            }

            // クリアしたらリザルトを表示
            if (clear) {
                resultPanel.GetComponent<Result>().SetData(player.GetComponent<Player>().Score);
                resultPanel.SetActive(true);
                scenarioState = ScenarioPatternType.START;
                Pose = !Pose;
                clear = false;
            }
        }
    }

    /// <summary>
    /// シナリオ読み込み命令
    /// </summary>
    private void CommandReadScenario() {
        // 既に読んでいたら返す
        if (GetScenarioData(scenarioState).IsRead) return;
        
        // シナリオデータをセット
        scenario = new ReadScenario(GetScenarioData(scenarioState).Scenario, scenarioObject, m_BGM);
        GetScenarioData(scenarioState).IsRead = true;

        // シナリオを読む際に必要なフラグをセット
        scenarioObject.SetActive(true); 
        Pose = false;
        stopEscKey = true;

        // 状態によって演出のフラグを立てる
        switch (scenarioState) {
            case ScenarioPatternType.BOSS:
            case ScenarioPatternType.WIN:
                StartAnim.SetBool("Boss", true);
                break;
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
    /// リスタートボタンを押す
    /// </summary>
    public void OnClickRestartButton() {
        player.GetComponent<Player>().Restart();
    }
}
