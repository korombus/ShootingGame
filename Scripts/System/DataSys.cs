using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DataSys {

    const string SAVE_KEY_OPTION_BGM_VOLUME = "save_key_option_bgm_volume"; //!< bgm音量のキー
    const string SAVE_KEY_OPTION_SE_VOLUME = "save_key_option_se_volume";   //!< se音量のキー

    private static float _bgmVolume;    //!< BGM音量
    private static float _seVolume;     //!< SE音量

    /// <summary>
    /// ロード
    /// </summary>
    protected static void Load() {
        
    }

    /// <summary>
    /// セーブ
    /// </summary>
    protected static void Save() {
        
    }

    /// <summary>
    /// オプション情報をロード
    /// </summary>
    protected static void LoadOptionData() {
        OPTION.SetVolume(PlayerPrefs.GetFloat(SAVE_KEY_OPTION_BGM_VOLUME), OPTION.Sound.BGM);
        OPTION.SetVolume(PlayerPrefs.GetFloat(SAVE_KEY_OPTION_SE_VOLUME), OPTION.Sound.SE);
    }

    /// <summary>
    /// オプション情報をセーブ
    /// </summary>
    protected static void SaveOptionData() {
        PlayerPrefs.SetFloat(SAVE_KEY_OPTION_BGM_VOLUME, OPTION.BGMVolume);
        PlayerPrefs.SetFloat(SAVE_KEY_OPTION_SE_VOLUME, OPTION.SEVolume);
    }
}

public class Data {

    /// <summary>
    /// ステージ名一覧
    /// </summary>
    public static List<string> d_StageNameList = new List<string>()
    {
        "none",
        "title",
        "opening",
        "stage1",
        "stage2"
    };

    /// <summary>
    /// ステージ毎にBGM
    /// </summary>
    public static Dictionary<Stage, string> d_StageBgmTable = new Dictionary<Stage, string>()
    {
        {Stage.title, "Title"},
        {Stage.stage1, "City_cry"},
        {Stage.stage2, "Dodger_direction"},
        {Stage.stage3, "Energy_emit"},
        {Stage.stage4, "Fake_fraction"},
        {Stage.stage5, "Glorius_glitter"},
        {Stage.stage6, "Heroic_hatred"}
    };

    /// <summary>
    /// SE一覧
    /// </summary>
    public static Dictionary<SoundEffectType, string> d_SoundEffect = new Dictionary<SoundEffectType, string>()
    {
        {SoundEffectType.SETUP_BUTTON, "button3"},
        {SoundEffectType.TITLE_BUTTON, "button4"},
        {SoundEffectType.BOOM,      "explosion6"},
        {SoundEffectType.PLAYER_SHOOT,  "shoot7"},
        {SoundEffectType.PLAYER_LASER, "laser8"},
    };

    public static Dictionary<SoundPatternCriAtom, string> d_CriCueSheetNameTable = new Dictionary<SoundPatternCriAtom, string>()
    {
        {SoundPatternCriAtom.BGM,       "BGM"},
        {SoundPatternCriAtom.AMBIENT,   "Ambient"},
        {SoundPatternCriAtom.EFFECT,    "Effect"},
        {SoundPatternCriAtom.MUSICAL,   "Musical"},
        {SoundPatternCriAtom.UI,        "UI"},
    };
}