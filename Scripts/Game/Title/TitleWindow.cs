using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleWindow : YkSys {

    // 外部変数
    public GameObject titleObj;     //!< タイトル表示オブジェクト
    public GameObject optionObj;    //!< オプション用オブジェクト
    public GameObject setupObj;     //!< 強化用オブジェクト
    public GameObject help;         //!< ヘルプオブジェクト
    public Slider BGMSlider;        //!< bgm音量用スライダー
    public Slider SESlider;         //!< se音量用スライダー
    public CommonSoundForCri bgmSound;    //!< bgm再生オブジェクト
    public Image titleBg;           //!< タイトル背景
    public Sprite enhanceBgTexture;

    // 内部変数
    private bool gameStart = false; //!< ゲームスタートフラグ

    public override void Awake() {
        base.Awake();

        titleObj.SetActive(true);
        optionObj.SetActive(false);
        setupObj.SetActive(false);

        // 既読フラグを全てなかったことにする
        scenarioData.ForEach(read => read.IsRead = false);
    }

    void Start() {
        // 音量を設定
        OPTION.SetVolume(OPTION.BGMVolume, OPTION.Sound.BGM, bgmSound);
        OPTION.SetVolume(OPTION.SEVolume, OPTION.Sound.SE);
    }

    void Update() {
        // ゲームが始まったらフェードを始める
        if (gameStart) {
            if (titleBg.color.r > 0) {
                Color color = titleBg.color;
                color.r -= 0.01f;
                color.g -= 0.01f;
                color.b -= 0.01f;
                color.a += 0.01f;
                titleBg.color = color;
            }
            else {
                if (!setupObj.activeSelf) {
                    // セットアップへ
                    setupObj.SetActive(true);
                    // 音楽再生
                    bgmSound.Play("Analyze");
                    titleBg.sprite = enhanceBgTexture;
                    Color color = titleBg.color;
                    color.r = 255f;
                    color.g = 255f;
                    color.b = 255f;
                    titleBg.color = color;
                }

                // 徐々に音を大きくする
                if (BGMSlider.value <= OPTION.BGMVolume) {
                    BGMSlider.value += 0.01f;
                }
                else {
                    YkSys.playerParam = new Player.PlayerEnhanceParam(1);
                    gameStart = false;
                }
            }
        }
    }

    /// <summary>
    /// ゲーム本体移動
    /// </summary>
    public void OnClickGoPlay() {
        SoundButton(Data.d_SoundEffect[SoundEffectType.TITLE_BUTTON]);
        // オープニングへ移動
        if (scenarioSkip) {
            Application.LoadLevel(1);
        } else {
        // シナリオがスキップされている場合はそのままステージへ
            Application.LoadLevel(2);
        }
    }

    /// <summary>
    /// ゲームスタートボタン
    /// </summary>
    public void OnClickGameStart() {
        SoundButton(Data.d_SoundEffect[SoundEffectType.TITLE_BUTTON], Data.d_CriCueSheetNameTable[SoundPatternCriAtom.UI]);
        titleObj.SetActive(false);
        optionObj.SetActive(false);
        help.GetComponent<MoveHelp>().SetData(HelpEnd);
    }

    public bool HelpEnd() {
        gameStart = true;
        bgmSound.Stop();
        return true;
    }

    /// <summary>
    /// オプション開始ボタン
    /// </summary>
    public void OnClickStartOption() {
        SoundButton(Data.d_SoundEffect[SoundEffectType.TITLE_BUTTON], Data.d_CriCueSheetNameTable[SoundPatternCriAtom.UI]);
        BGMSlider.value = OPTION.BGMVolume;
        SESlider.value = OPTION.SEVolume;
        titleObj.SetActive(false);
        optionObj.SetActive(true);
        // デバッグ状態でないならばデバッグ用のオブジェクトは全て止める
        if (!Debug.isDebugBuild) {
            CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).SetActive(false);
        }
        else {
            CommonUtil.SearchObjectChild("MutekiToggle", CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).transform).GetComponent<Toggle>().onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            CommonUtil.SearchObjectChild("MutekiToggle", CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).transform).GetComponent<Toggle>().isOn = YkSys.mutekiFlag;
            CommonUtil.SearchObjectChild("MutekiToggle", CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).transform).GetComponent<Toggle>().onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);

            CommonUtil.SearchObjectChild("ScenarioSkipToggle", CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).transform).GetComponent<Toggle>().onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            CommonUtil.SearchObjectChild("ScenarioSkipToggle", CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).transform).GetComponent<Toggle>().isOn = YkSys.scenarioSkip;
            CommonUtil.SearchObjectChild("ScenarioSkipToggle", CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).transform).GetComponent<Toggle>().onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);

            CommonUtil.SearchObjectChild("BattleSkipToggle", CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).transform).GetComponent<Toggle>().onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
            CommonUtil.SearchObjectChild("BattleSkipToggle", CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).transform).GetComponent<Toggle>().isOn = YkSys.battleSkip;
            CommonUtil.SearchObjectChild("BattleSkipToggle", CommonUtil.SearchObjectChild("DebugObject", optionObj.transform).transform).GetComponent<Toggle>().onValueChanged.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.RuntimeOnly);
        }
    }

    /// <summary>
    /// オプション終了ボタン
    /// </summary>
    public void OnClickEndOption() {
        SoundButton(Data.d_SoundEffect[SoundEffectType.TITLE_BUTTON]);
        OPTION.SaveOption();
        titleObj.SetActive(true);
        optionObj.SetActive(false);
    }

    public void SoundButton(string cueName, string cueSheetName = null) {
        if (!string.IsNullOrEmpty(cueSheetName)) {
            bgmSound.SetCueSheetName(cueSheetName, true);
        }
        bgmSound.Play(cueName, true);
    }

    /// <summary>
    /// BGM音量変更
    /// </summary>
    /// <param name="val">BGM音量</param>
    public void ChangeBGMVolume(float val) {
        OPTION.SetVolume(val, OPTION.Sound.BGM, bgmSound);
    }

    /// <summary>
    /// SE音量変更
    /// </summary>
    /// <param name="val"></param>
    public void ChangeSEVolume(float val) {
        OPTION.SetVolume(val, OPTION.Sound.SE);
    }

    /// <summary>
    /// シナリオスキップデバッグ
    /// </summary>
    public void OnClickScenarioSkip() {
        scenarioSkip = !scenarioSkip;
    }

    /// <summary>
    /// 無敵モードデバッグ
    /// </summary>
    public void OnClickMutekiMode() {
        mutekiFlag = !mutekiFlag;
    }

    /// <summary>
    /// バトルスキップデバッグ
    /// </summary>
    public void OnClickBattleSkip() {
        battleSkip = !battleSkip;
    }
}
