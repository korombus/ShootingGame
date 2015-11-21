using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BossEnemyData {

    /// <summary>
    /// ボス敵隊列クラス
    /// </summary>
    public class BossEnemyFormation
    {
        public int trans_id;                //!< 生成位置ID
        public float pos;                   //!< 生成ポジション
        public float move_time;             //!< 動き出す時間
        public float scale;                 //!< 大きさ
        public int life;                    //!< 体力
        public int score;                   //!< スコア
        public GameObject prefab;           //!< 生成するボスのプレハブ
        public bool retreatFlag;            //!< 撤退フラグ
        public float retreatLife;           //!< 撤退体力
        public List<BossAI> boss_ai;        //!< ボスのAI 
        public BossEnemyFormation(int i_trans_id, float i_pos, float i_move_time, float i_scale, int i_life, int i_score, string prefabName, string i_retreat, float i_retreatLife, List<BossAI> i_ai) {
            trans_id = i_trans_id;
            pos = i_pos;
            move_time = i_move_time;
            scale = i_scale;
            life = i_life;
            score = i_score;
            prefab = Resources.Load<GameObject>("Prefabs/Enemy/Boss/" + prefabName);
            retreatFlag = i_retreat == "1";
            retreatLife = i_retreatLife;
            boss_ai = i_ai;
        }
    }

    /// <summary>
    /// ボスAIクラス
    /// </summary>
    public class BossAI
    {
        public int movePattern;             //!< 移動パターン
        public List<EnemyBulletBase.EnemyBulletGroup> bulletType = new List<EnemyBulletBase.EnemyBulletGroup>(); //!< 弾丸パターン
        public float bulletShotInterval;    //!< 弾丸発射間隔
        public int summon_enemy_form_id;    //!< 召喚する敵の隊列ID
        public float summon_interval;       //!< 召喚間隔
        public float speed;                 //!< 移動速度
        public int trans_id;                //!< 方向ID
        public float leap_time;             //!< 向き直り時間
        public float switch_time;           //!< 切り替え時間
        public BossAI(int i_move, int i_type, float i_interval, int i_form, float i_summonInter, float i_spd, int i_trans_id, float i_leap_time, float time) {
            movePattern = i_move;
            bulletType = EnemyBulletBase.getBulletGroupList(i_type);
            bulletShotInterval = i_interval;
            summon_enemy_form_id = i_form;
            summon_interval = i_summonInter;
            speed = i_spd;
            trans_id = i_trans_id;
            leap_time = i_leap_time;
            switch_time = time;
        }
    }

    public static List<string> getBossFormationLineStrDataListFromResorcesPath(string path) {
        TextAsset boss = Resources.Load<TextAsset>(path);
        return CommonUtil.InitLineData(boss.text.Split(new char[] { '\n', '\r' }).ToList<string>());
    }

    public static List<string> getBossFormationLineStrDataListFromStrData(string data) {
        return CommonUtil.InitLineData(data.Split(new char[] { '\n', '\r' }).ToList<string>());
    }

    public static void setBossFormationData(ref Dictionary<Stage, BossEnemyFormation> formData, List<string> data) {
        int stayAI = 0;
        List<BossAI> ai = new List<BossEnemyData.BossAI>();
        foreach (string line in data) {
            string[] bossData = line.Split(new char[] { ',', '\n', '\r' });
            CommonUtil.Unset(ref bossData);
            // ボスAIならばこっち
            if (int.TryParse(bossData[0], out stayAI)) {
                ai.Add(new BossEnemyData.BossAI(int.Parse(bossData[0]), int.Parse(bossData[1]), float.Parse(bossData[2]), int.Parse(bossData[3]), float.Parse(bossData[4]), float.Parse(bossData[5]), int.Parse(bossData[6]), float.Parse(bossData[7]), int.Parse(bossData[8])));
            }
            else {
                // ボスデータを作成
                formData.Add((CommonSys.GetSystem() as TopWindow).GetStringToStageEnum(bossData[0]), new BossEnemyFormation(int.Parse(bossData[1]), float.Parse(bossData[2]), float.Parse(bossData[3]), float.Parse(bossData[4]), int.Parse(bossData[5]), int.Parse(bossData[6]), bossData[7], bossData[8], float.Parse(bossData[9]), ai));
                ai = new List<BossEnemyData.BossAI>();
            }
        }
    }

    public static void setBossFormationDataForDebug(ref Dictionary<Stage, BossEnemyFormation> formData, List<string> data) {
        int stayAI = 0;
        List<BossAI> ai = new List<BossEnemyData.BossAI>();
        foreach (string line in data) {
            string[] bossData = line.Split(new char[] { ',', '\n', '\r' });
            CommonUtil.Unset(ref bossData);
            // ボスAIならばこっち
            if (int.TryParse(bossData[0], out stayAI)) {
                ai.Add(new BossEnemyData.BossAI(int.Parse(bossData[0]), int.Parse(bossData[1]), float.Parse(bossData[2]), int.Parse(bossData[3]), float.Parse(bossData[4]), float.Parse(bossData[5]), int.Parse(bossData[6]), float.Parse(bossData[7]), int.Parse(bossData[8])));
            }
            else {
                // ボスデータを作成
                formData.Add((CommonSys.GetSystem() as BattleDebug).GetStringToStageEnum(bossData[0]), new BossEnemyFormation(int.Parse(bossData[1]), float.Parse(bossData[2]), float.Parse(bossData[3]), float.Parse(bossData[4]), int.Parse(bossData[5]), int.Parse(bossData[6]), bossData[7], bossData[8], float.Parse(bossData[9]), ai));
                ai = new List<BossEnemyData.BossAI>();
            }
        }
    }
}
