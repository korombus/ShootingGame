using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ReadScenario {

    // 音楽のフェードをコントロールするクラス
    internal class BGMFadeController
    {
        public int id;             //!< フェードID(0:イン1, 1:アウト1, 2:イン2, 3:アウト2)
        public bool fadeInFlag;    //!< フェードインしたかのフラグ
        public float speed;        //!< フェード速度

        internal BGMFadeController(int i_id, bool i_flag, float i_speed){
            id = i_id;
            fadeInFlag = i_flag;
            speed = i_speed;
        }
    }

    // 定数
    private const string CHARACTER_IMAGE_OBJECT_NAME    = "CharaImage";        //!< キャラクター画像表示用オブジェクト
    private const string CHARACTER_NAME_OBJECT          = "CharacterNameText";  //!< キャラクター名表示用オブジェクト 
    private const string FRAME_TEXT_OBJECT              = "FrameText";          //!< 台詞表示用オブジェクト  
    private const string BG_IMAGE_OBJECT_NAME           = "BgImage";

    private const string CHARACTER_SPRITE_ROOT          = "Sprites/Character/";  //!< キャラクタースプライトまでのルート
    private const string CHARACTER_SPRITE_NAME          = "characterSprite";     //!< 包括スプライト名

    private const string CHARACTER_SPRITE_PLAYER_NAME   = "Player_";    //!< スプライト内プレイヤー名
    private const string CHARACTER_SPRITE_VU_NAME       = "VU_";        //!< スプライト内VU名

    private const string PLAY_BGM_OBJECT_NAME           = "Sound";        //!< BGM再生用オブジェクト名
    private const string PLAY_SE_OBJECT_NAME            = "Sound";         //!< SE再生用オブジェクト名

    // 内部変数
    private float timer = 0f;           //!< 読み出しタイマー
    private float speed = 0.6f;         //!< 読み出し速度
    private float waitTime = 0f;        //!< 読み出し一時停止時間
    private float waitTimer = 0f;       //!< 読み出し一時停止タイマー

    private bool endState = true;       //!< 読み終わり状態

    private CommonSoundForCri bgmObject;      //!< bgm再生用オブジェクト

    private TextAsset scenarioText;     //!< 読み出すシナリオテキスト
    private StateReadScenario state;    //!< シナリオの読み出し状態

    private Text frameText;             //!< テキスト表示用フレーム
    private Text charaNameText;         //!< キャラ名表示テキスト

    private Image charaObj;             //!< キャラクターオブジェクト

    private List<Sprite> charaSprite = new List<Sprite>();  //!< キャラクタースプライト

    private RawImage bgImage;           //!< 背景オブジェクト

    private List<string> lineText = new List<string>();     //!< 一行テキスト
    private int lineIndex = 0;                              //!< 一行テキストのインデックス

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="txt">テキストデータ</param>
    public ReadScenario(TextAsset txt, GameObject scenarioObj, CommonSoundForCri bgm, string debugtxt = null) {
        // シナリオのテキストをセット
        scenarioText = txt;
        state = StateReadScenario.BEGINE;

        lineText.Clear();
        // シナリオを一行ずつに切り分け
        if (scenarioText == null) { CreateLineText(debugtxt); }
        else { CreateLineText(scenarioText); }

        endState = false;
        timer = 0f;

        // bgm用コンポーネント
        bgmObject = bgm;
        bgmObject.ChangeVolume(OPTION.BGMVolume);

        // テキスト表示用フレームセット
        frameText = CommonUtil.SearchObjectChild(FRAME_TEXT_OBJECT, scenarioObj.transform).GetComponent<Text>();
        frameText.text = "";

        // キャラ名表示用テキスト
        charaNameText = CommonUtil.SearchObjectChild(CHARACTER_NAME_OBJECT, scenarioObj.transform).GetComponent<Text>();
        charaNameText.text = "";

        // キャラ表示用オブジェクト
        charaObj = CommonUtil.SearchObjectChild(CHARACTER_IMAGE_OBJECT_NAME, scenarioObj.transform).GetComponent<Image>();

        // キャラスプライトがなければ探索
        if (!charaSprite.Exists(spr => spr.name == CHARACTER_SPRITE_PLAYER_NAME + "0")) {
            Sprite[] chara = Resources.LoadAll<Sprite>(CHARACTER_SPRITE_ROOT + CHARACTER_SPRITE_NAME);
            foreach (Sprite spr in chara) {
                charaSprite.Add(spr);
            }
        }

        // 最初の画像はここで読み込んでおく(その行は二重で読んでも問題ないので消さない)
        foreach (string img in lineText) {
            string[] disp = img.Split(new char[] { ' ' });
            if (StrToSharpCommand(disp[0].Substring(1)) == ScenarioSharpCommand.DISP || StrToSharpCommand(disp[0].Substring(1)) == ScenarioSharpCommand.DISPCLEAR) {
                Command(disp, ScenarioCommandType.SHARP);
                break;
            }
        }

        // 背景画像表示用イメージ
        bgImage = CommonUtil.SearchObjectChild(BG_IMAGE_OBJECT_NAME, scenarioObj.transform).GetComponent<RawImage>();
        bgImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// シナリオを読み出す
    /// </summary>
    /// <returns></returns>
    private int wordIndex = 0;      // 現在の文字位置
    private int colorCodeLen = 0;   // 色の指定の文字の長さ
    public bool ReadScenarioText() {
        switch (state) {
            case StateReadScenario.BEGINE:
                // 読み出し開始
                state = StateReadScenario.READ;
                break;

            case StateReadScenario.READ:
                // ここを通る場合は全て待機させる
                if (lineText[lineIndex].Length <= wordIndex) {
                    // まだ文があれば一時停止する
                    if (lineText.Count > lineIndex + 1) {
                        lineIndex++;
                        wordIndex = 0;
                        state = StateReadScenario.STOP;
                    }
                    else {
                        // 全部読んだら終わり
                        state = StateReadScenario.END;
                    }
                }
                else {
                    // コマンド読み込み
                    if (lineText[lineIndex].Substring(0, 1) == "#") {
                        Command(lineText[lineIndex].Split(new char[] { ' ' }), ScenarioCommandType.SHARP);
                        lineIndex++;
                        // 最後がコマンドで終わったらそのまま開始する
                        if (lineIndex >= lineText.Count) {
                            state = StateReadScenario.NONE;
                            endState = true;
                        }
                    }

                    // 文字の色変え事前処理
                    else if (lineText[lineIndex].Substring(wordIndex, 1) == "<" && lineText[lineIndex].Substring(wordIndex, 2) == "<@") {
                        // 最初の'>'までが色指定なのでそこを保持
                        int len = 2;
                        List<string> com = new List<string>() { "<color>" };
                        // カラーコード抜き出し
                        while (true) {
                            if (lineText[lineIndex].Substring(wordIndex + len + 2, 1) == ">") {
                                colorCodeLen = len;
                                com.Add(lineText[lineIndex].Substring(wordIndex + 2, len));
                                break;
                            }
                            len++;
                        }

                        int bodyLen = 1;
                        // カラーコードで囲われた本文の抜き出し
                        while (true) {
                            if (lineText[lineIndex].Substring(wordIndex + colorCodeLen + 3 + bodyLen, 1) == "<") {
                                com.Add(lineText[lineIndex].Substring(wordIndex + colorCodeLen + 3, bodyLen));
                                break;
                            }
                            bodyLen++;
                        }
                        // コマンド実行
                        Command(com.ToArray(), ScenarioCommandType.LINECOM);
                    }

                    // 文字の色変え
                    else if (lineText[lineIndex].Substring(wordIndex, 1) == "<" && lineText[lineIndex].Substring(wordIndex, 2) == "<c") {
                        if (timer >= speed) {
                            //「<color=>W</color>」とカラーコード分文字を読み込む進める
                            wordIndex += colorCodeLen + 17;
                            frameText.text = lineText[lineIndex].Substring(0, wordIndex);
                        }
                        timer += Time.deltaTime;
                    }

                    // 台詞読み込み
                    else if (lineText[lineIndex].Substring(0, 2) != "//" && lineText[lineIndex].Substring(0, 1) != "@") {
                        // 一行内のコマンドに当たったらコマンド実行
                        if (lineText[lineIndex].Substring(wordIndex, 1) == "[") {
                            Command(new string[] { lineText[lineIndex].Substring(wordIndex, 3) }, ScenarioCommandType.LINECOM);
                        }
                        // それ以外は普通に表示
                        else {
                            if (timer >= speed) {
                                wordIndex++;
                                frameText.text = lineText[lineIndex].Substring(0, wordIndex);
                            }
                            timer += Time.deltaTime;
                        }
                        // 文の終わりに来たら
                        if (wordIndex >= lineText[lineIndex].Length) {
                            // まだ文があれば一時停止する
                            if (lineText.Count > lineIndex + 1) {
                                state = StateReadScenario.STOP;
                            }
                            else {
                                // 全部読んだら終わり
                                state = StateReadScenario.END;
                            }
                        }
                    }

                    // コメントや事前処理は全て飛ばす
                    else {
                        lineIndex++;
                    }
                }
                break;

            case StateReadScenario.WAIT:
                if (waitTimer > waitTime) {
                    waitTimer = 0f;
                    state = StateReadScenario.READ;
                }
                waitTimer += state == StateReadScenario.WAIT ? Time.deltaTime : 0;
                break;

            case StateReadScenario.STOP:
                LineEndSign();
                break;
        }
        // 読み出し速度を初期化
        speed = 0.6f;

        return endState;
    }

    /// <summary>
    /// 画面がクリックされたら
    /// </summary>
    public void OnClickDisplay() {
        // 読み出し最中のみ速度を上げる
        speed = (state == StateReadScenario.READ) ? 0.2f : 0.6f;
        if (state == StateReadScenario.STOP) {
            wordIndex = 0;
            lineIndex++;
            state = StateReadScenario.READ;
        }
        if (state == StateReadScenario.END) {
            state = StateReadScenario.NONE;
            endState = true;
        }
    }

    /// <summary>
    /// 一行テキストを取得
    /// </summary>
    /// <param name="index">インデックス</param>
    /// <param name="line">一行まとめ</param>
    /// <returns></returns>
    private void CreateLineText(TextAsset txt){
        lineText = txt.text.Split(new char[] { '\n', '\r' }).ToList<string>();
        lineText.RemoveAll(none => none == "\n" || none == "\r" || none == "");
    }

    private void CreateLineText(string txt) {
        lineText = txt.Split(new char[] { '\n', '\r' }).ToList<string>();
        lineText.RemoveAll(none => none == "\n" || none == "\r" || none == "");
    }

    /// <summary>
    /// コマンド
    /// </summary>
    /// <param name="command">コマンドデータ</param>
    private Dictionary<string, Sprite> cacheData = new Dictionary<string, Sprite>();
    private void Command(string[] command, ScenarioCommandType type) {
        switch (type) {
            case ScenarioCommandType.SHARP:

                ScenarioSharpCommand com = StrToSharpCommand(command[0].Substring(1));
                switch (com) {

                    case ScenarioSharpCommand.DISP:
                        // キャラ名
                        charaNameText.text = command[1];

                        // コマンドの長さによって処理を変更
                        if (command.Length >= 3) {
                            // キャッシュデータがあるならば上書き
                            if (cacheData.ContainsKey(command[1])) {
                                cacheData[command[1]] = charaSprite.Find(spr => spr.name == command[2]);
                            }
                            // なければ作成
                            else {
                                cacheData.Add(command[1], charaSprite.Find(spr => spr.name == command[2]));
                            }
                        }

                        // 画像表示
                        if (cacheData.Count >= 1) {
                            charaObj.enabled = true;
                            charaObj.sprite = cacheData[command[1]];
                        }
                        break;

                    case ScenarioSharpCommand.DISPCLEAR:
                        if (command.Length >= 2) { charaNameText.text = command[1]; }
                        else { charaNameText.text = ""; }
                        charaObj.enabled = false;
                        break;

                    case ScenarioSharpCommand.BGM:
                        // コマンドで強制フェードされている場合は以降bgmは流さない
                        bgmObject.Stop();
                        //bgmObject.CueSheetNameBGM = command[1];
                        //bgmObject.Play(command[2]);
                        break;

                    case ScenarioSharpCommand.SE:
                        //bgmObject.CueSheetNameSE = command[1];
                        //bgmObject.Play(command[2], true);
                        break;

                    case ScenarioSharpCommand.BGIMG:
                        if (command.Length == 1) { bgImage.gameObject.SetActive(false); }
                        else {
                            bgImage.gameObject.SetActive(true);
                            bgImage.texture = Resources.Load<Texture>("Textures/" + command[1]);
                        }
                        break;

                    case ScenarioSharpCommand.WAIT:
                        if (!float.TryParse(command[1], out waitTime)) {
                            waitTime = 0f;
                        }
                        Debug.Log(waitTime);
                        state = StateReadScenario.WAIT;
                        break;

                }

                break;

            case ScenarioCommandType.LINECOM:
                // 一時停止
                if (command[0] == "[l]") {
                    // 一時停止コマンドを文字から削除
                    lineText[lineIndex] = lineText[lineIndex].Replace("[l]", "");
                    state = StateReadScenario.STOP;
                }

                // 改行
                if (command[0] == "[r]") {
                    // コマンドを改行に入れ替え
                    lineText[lineIndex] = lineText[lineIndex].Replace("[r]", Environment.NewLine);
                    // 次の行を連結
                    lineText[lineIndex] += lineText[lineIndex + 1];
                    // 次の行を削除
                    lineText.RemoveAt(lineIndex + 1);
                }

                // コマンド改行
                if (command[0] == "[cr") {
                    // 次の行の#コマンドをここで読む
                    Command(lineText[lineIndex + 1].Split(' '), ScenarioCommandType.SHARP);
                    // #コマンド行を削除
                    lineText.RemoveAt(lineIndex + 1);
                    // コマンドを改行に入れ替え
                    lineText[lineIndex] = lineText[lineIndex].Replace("[cr]", Environment.NewLine);
                    // 次々行を連結
                    lineText[lineIndex] += lineText[lineIndex + 1];
                    // 次々行も削除
                    lineText.RemoveAt(lineIndex + 1);
                }

                // 色替えコマンド
                if (command[0] == "<color>") {
                    string body = null;
                    for (int i = 0; i < command[2].Length; i++) {
                        // 正式なコマンドを生成して文字に割り当てる
                        body += "<color=" + command[1] + ">" + command[2].Substring(i, 1) + "</color>";
                    }
                    // 本文にあるコマンド部分を消す
                    lineText[lineIndex] = lineText[lineIndex].Replace("<@" + command[1] + ">", "");
                    lineText[lineIndex] = lineText[lineIndex].Remove(lineText[lineIndex].IndexOf("</>"), 3);
                    // 本文とコマンドを置換する
                    lineText[lineIndex] = lineText[lineIndex].Replace(command[2], body);
                }
                break;
        }
    }

    /// <summary>
    /// #コマンドを列挙型に変換する
    /// </summary>
    /// <param name="command">コマンド文字列</param>
    /// <returns>コマンド列挙タイプ</returns>
    private ScenarioSharpCommand StrToSharpCommand(String command) {
        Debug.Log(command);
        ScenarioSharpCommand com = ScenarioSharpCommand.NONE;
        try {
            com = (ScenarioSharpCommand)Enum.Parse(typeof(ScenarioSharpCommand), command.ToUpper());
        }catch(Exception){}
        return com;
    }

    bool sign = false;
    float signTimer = 0f;
    private void LineEndSign(bool end = false) {
        if (!sign && !end && signTimer >= 0.5f) {
            frameText.text = lineText[lineIndex].Substring(0, wordIndex) + "<color=red>∇</color>";
            sign = true;
            signTimer = 0f;
        }
        else {
            if (signTimer >= 0.5f) {
                frameText.text = lineText[lineIndex].Substring(0, wordIndex);
                sign = false;
                signTimer = 0f;
            }
        }
        signTimer += Time.deltaTime;
    }
}
