using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class SoundDebug : MonoBehaviour {
    /*
    private const string PLAY_BGM_OBJECT_NAME = "BGM";        //!< BGM再生用オブジェクト名
    private const string PLAY_SE_OBJECT_NAME = "SE";         //!< SE再生用オブジェクト名

    // 音楽のフェードをコントロールするクラス
    internal class BGMFadeController
    {
        public int id;             //!< フェードID(0:イン1, 1:アウト1, 2:イン2, 3:アウト2)
        public bool fadeInFlag;    //!< フェードインしたかのフラグ
        public float speed;        //!< フェード速度
        public CommonSoundForCri useSoundObject;    //!< 使用するサウンドObject

        internal BGMFadeController(int i_id, bool i_flag, float i_speed) {
            id = i_id;
            fadeInFlag = i_flag;
            speed = i_speed;
        }
    }

    private List<BGMFadeController> fadeController = new List<BGMFadeController>(){
        new BGMFadeController(0, false, 0.0f),
        new BGMFadeController(1, false, 0.0f),
        new BGMFadeController(2, false, 0.0f),
        new BGMFadeController(3, false, 0.0f),
    };


    // 外部変数
    public CommonSoundForCri bgmObject;
    public GameObject buttonObject;
    public GameObject panel;

    // 内部データ
    private bool commandBgmFade;

    void Start() {
        StartCoroutine(ReadComboBGMData());
    }

    /// <summary>
    /// 音源組み合わせデータを読み込む
    /// </summary>
    /// <returns></returns>
    IEnumerator ReadComboBGMData() {
        TextAsset textData = Resources.Load<TextAsset>("Sounds/ComboBGM");
        List<string> text = CommonUtil.ArrayToList<string>(textData.text.Split(new char[] { '\n', '\r' }));
        CommonUtil.Unset(ref text);
        text.RemoveAll(rem => rem.Substring(0, 2) == "//");
        yield return null;
    }

    public void OnClickButton(GameObject obj) {
        Debug.Log(obj.name);
    }

    public void BeginDebug(string command) {
        commandBgmFade = true;
    }
     * */
}
