using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BackScrollBase : MonoBehaviour {

    public StageBackgroundType type;
    public int id = 0;

    private float timer = 0f;
    List<float> TimeList = new List<float>();

    public bool stopOpenFlag { private set; get; }
    void Start() {
        TimeList = StageBackground.GetBgImageSettingListGroupByStageType().Select((value, index) => value.bgSetSwitchTime).ToList();
        SetStopOpenFlag(StageBackground.GetBgImageSettingListGroupByStageType()[0].bgSetStopStartFlag);
    }

    protected void Update() {
        if (TimeList[id] < timer) {
            if (TimeList.Count - 1 > id) {
                id++;
                SetStopOpenFlag(StageBackground.GetBgImageSettingListGroupByStageType()[id].bgSetStopStartFlag);
            }
            timer = 0f;
        }
        timer += Time.deltaTime;
    }

    public void SetStopOpenFlag(bool flag) {
        stopOpenFlag = flag;
    }

    public void SetId(Func<Texture, bool> SetTexture, Func<float, bool> SetSpeed, int i_id) {
        SetTexture(GetTextureData(i_id, type));
        SetSpeed(GetSpeedData(i_id, type));
    }

    private Texture GetTextureData(int id, StageBackgroundType type) {
        List<StageBackground.BackgroundImage> list = StageBackground.GetBgImageListGroupByImgGroupId(id).Where(val => val.bgImgPosType == type).ToList();
        return list.Count > 1 ? list[UnityEngine.Random.Range(0, list.Count - 1)].bgImg : list.Count == 1 ? list[0].bgImg : null;
    }

    private float GetSpeedData(int id, StageBackgroundType type) {
        List<StageBackground.BackgroundImage> list = StageBackground.GetBgImageListGroupByImgGroupId(id).Where(val => val.bgImgPosType == type).ToList();
        return list.Count > 1 ? list[UnityEngine.Random.Range(0, list.Count - 1)].bgSpeed : list.Count == 1 ? list[0].bgSpeed : 0.1f;
    }
}
