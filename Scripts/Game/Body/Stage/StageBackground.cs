using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class StageBackground : MonoBehaviour{

    public class BackgroundImage
    {
        public Stage bgImgStage { private set; get; }
        public int bgImgGroupId { private set; get; }
        public StageBackgroundType bgImgPosType { private set; get; }
        public Texture bgImg { private set; get; }
        public float bgSpeed { private set; get; }
        public bool bgRandFlag { private set; get; }

        public BackgroundImage(Stage i_stg, int i_groupId, StageBackgroundType i_pos, Texture i_tex, float i_spd, bool i_flag) {
            bgImgStage      = i_stg;
            bgImgGroupId    = i_groupId;
            bgImgPosType    = i_pos;
            bgImg           = i_tex;
            bgSpeed         = i_spd;
            bgRandFlag      = i_flag;
        }
    }

    public class BackgroundImageSetting
    {
        public Stage bgSetStage { private set; get; }
        public int bgSetGroupId { private set; get; }
        public float bgSetSwitchTime { private set; get; }
        public bool bgSetStopStartFlag { private set; get; }

        public BackgroundImageSetting(Stage i_stg, int i_groupId, float i_time, bool i_flag) {
            bgSetStage          = i_stg;
            bgSetGroupId        = i_groupId;
            bgSetSwitchTime     = i_time;
            bgSetStopStartFlag  = i_flag;
        }
    }

    public static Dictionary<int, BackgroundImageSetting> stageBackgroundImageSettingTable = new Dictionary<int, BackgroundImageSetting>();
    public static Dictionary<int, BackgroundImage> stageBackgroundImageTable = new Dictionary<int, BackgroundImage>();

    public static void CreateBackgroundImageData(string bgImgDataStr = null) {
        if (bgImgDataStr == null) {
            if (stageBackgroundImageTable.Count > 0) return;
            TextAsset bgData = Resources.Load<TextAsset>("Textures/MasterData/StageBgImgData");
            if (bgData != null) {
                List<string> groupList = CommonUtil.GetCsvTextToList(bgData);
                // 背景データ作成
                foreach (var data in groupList) {
                    string[] group = data.Split(new char[] { ',' });
                    stageBackgroundImageTable.Add(int.Parse(group[0]), new BackgroundImage((Stage)int.Parse(group[1]) + 3, int.Parse(group[2]), (StageBackgroundType)int.Parse(group[3]), Resources.Load<Texture>("Textures/Stage/" + ((Stage)int.Parse(group[1]) + 3) + "/" + group[2] + "/" + group[4]), float.Parse(group[5]), group[6].Equals("1")));
                }
            }
        } else {
            List<string> groupList = CommonUtil.ArrayToList(bgImgDataStr.Split(new char[] { '\n' }));
            foreach (var data in groupList) {
                string[] group = data.Split(new char[] { ',' });
                stageBackgroundImageTable.Add(int.Parse(group[0]), new BackgroundImage((Stage)int.Parse(group[1]) + 3, int.Parse(group[2]), (StageBackgroundType)int.Parse(group[3]), Resources.Load<Texture>("Textures/Stage/" + ((Stage)int.Parse(group[1]) + 3) + "/" + group[2] + "/" + group[4]), float.Parse(group[5]), group[6].Equals("1")));
            }
        }
    }

    public static void CreateBackgroundImageSettingData(string bgImgSetDataStr = null) {
        if (bgImgSetDataStr == null) {
            if (stageBackgroundImageSettingTable.Count > 0) return;
            TextAsset bgData = Resources.Load<TextAsset>("Textures/MasterData/StageBgImgSettingData");
            if (bgData != null) {
                List<string> groupList = CommonUtil.GetCsvTextToList(bgData);
                // 背景設定データ作成
                foreach (var data in groupList) {
                    string[] group = data.Split(new char[] { ',' });
                    stageBackgroundImageSettingTable.Add(int.Parse(group[0]), new BackgroundImageSetting((Stage)int.Parse(group[1])+3, int.Parse(group[2]), float.Parse(group[3]), group[4].Equals("1")));
                }
            }
        }
        else {
            List<string> groupList = CommonUtil.ArrayToList(bgImgSetDataStr.Split(new char[] { '\n' }));
            foreach (var data in groupList) {
                string[] group = data.Split(new char[] { ',' });
                stageBackgroundImageSettingTable.Add(int.Parse(group[0]), new BackgroundImageSetting((Stage)int.Parse(group[1])+3, int.Parse(group[2]), float.Parse(group[3]), group[4].Equals("1")));
            }
        }
    }

    public static List<BackgroundImageSetting> GetBgImageSettingListGroupByStageType() {
        return (from s in stageBackgroundImageSettingTable
                where s.Value.bgSetStage == YkSys.nowStage
                select s.Value).ToList();
    }

    public static List<BackgroundImage> GetBgImageListGroupByStageType() {
        return (from s in stageBackgroundImageTable
                where s.Value.bgImgStage == YkSys.nowStage
                select s.Value).ToList();
    }

    public static List<BackgroundImage> GetBgImageListGroupByImgGroupId(int groupId) {
        return (from s in stageBackgroundImageTable
                where s.Value.bgImgGroupId == groupId
                where s.Value.bgImgStage == YkSys.nowStage
                select s.Value).ToList();
    }
}
