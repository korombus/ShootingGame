using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CommonSoundForCri : MonoBehaviour {

    // 定数
    public const string BGMPath    = "Sounds/BGM/";
    public const string SEPath     = "Sounds/SE/";

    // 外部変数
    public string CueSheetNameBGM = "";
    public string CueSheetNameSE = "";

    public List<CriAtomSource> criAtomList = new List<CriAtomSource>();

    void Awake() {
        this.gameObject.GetComponents<CriAtomSource>(criAtomList);
    }

    /*******************************************************/
    /* !@brief  : CueSheet名を設定
     *  @param[in]  : sheetName  -> 登録するシート名
     *  @retval : なし
     *  @date   : 2015/07/06
     *  @author : コロソブス(korombus)
     *******************************************************/
    public void SetCueSheetName(string sheetName, bool se = false) {
        if (sheetName != null) {
            if (se) {
                CueSheetNameSE = sheetName;
            }
            else {
                CueSheetNameBGM = sheetName;
            }
        }
        else {
            Debug.Log("No Cue Sheet Name");
        }
    }

    /*******************************************************/
    /* !@brief  : BGM再生
     *  @param[in]  : clip      -> 流したい音楽データ
     *  @retval : なし
     *  @date   : 2014/03/12
     *  @author : コロソブス(korombus)
     *******************************************************/
    public void Play(string cueName, bool se = false) {
        CriAtomSource source;
        if (se) {
            if (CueSheetNameSE == "") return;
            source = criAtomList[1];
            source.cueSheet = CueSheetNameSE;
        }
        else {
            if (CueSheetNameBGM == "") return;
            source = criAtomList[0];
            source.cueSheet = CueSheetNameBGM;
        }
        source.Play(cueName);
    }


    /*******************************************************/
    /* !@brief  : 音量調整
     *  @param[in]  : value      -> 音量
     *  @retval : なし
     *  @date   : 2014/03/12
     *  @author : コロソブス(korombus)
     *******************************************************/
    public void ChangeVolume(float value, bool se = false) {
        if (value > 1.0f || value < 0) {
            value = 1.0f;
        }
        if(se){
            criAtomList[1].volume = value;
        }else{
            criAtomList[0].volume = value;
        }
    }

    /*******************************************************/
    /* !@brief  : 音楽停止
     *  @param[in]  : なし
     *  @retval : なし
     *  @date   : 2014/03/13
     *  @author : コロソブス(korombus)
     *******************************************************/
    public void Stop(bool se = false) {
        GetComponentInCriAtomSource(se).Stop();
    }

    /*******************************************************/
    /* !@brief  : 音楽一時停止
     *  @param[in]  : なし
     *  @retval : なし
     *  @date   : 2015/02/26
     *  @author : コロソブス(korombus)
     *******************************************************/
    public void Pause(bool se = false) {
        GetComponentInCriAtomSource(se).Pause(!GetComponentInCriAtomSource(se).IsPaused());
    }

    private CriAtomSource GetComponentInCriAtomSource(bool se) {
        return se ? criAtomList[1] : criAtomList[0];
    }
}
