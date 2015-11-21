using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NormalEnemyData {

	/// <summary>
    /// ザコ敵隊列クラス
    /// </summary>
    public class EnemyFormation
    {
        public int id;            //!< 隊列ID
        public int trans_id;      //!< 生成位置ID
        public float pos;         //!< 生成ポジション
        public float move_time;   //!< 動き出す時間
        public float scale;       //!< 大きさ
        public int life;          //!< 体力
        public int score;         //!< スコア
        public GameObject prefab; //!< 生成する敵のプレハブ
        public List<NormalEnemyBulletAI> enemyAI = new List<NormalEnemyBulletAI>();    //!< 通常敵AI
        public EnemyFormation(int i_id, int i_trans_id, float i_pos, float i_move_time, float i_scale, int i_life, int i_score, string prefabName, List<NormalEnemyBulletAI> i_ai) {
            id = i_id;
            trans_id = i_trans_id;
            pos = i_pos;
            move_time = i_move_time;
            scale = i_scale;
            life = i_life;
            score = i_score;
            enemyAI = i_ai;
            prefab = Resources.Load<GameObject>("Prefabs/Enemy/Normal/" + prefabName);
        }
    }

    /// <summary>
    /// ザコ敵AIクラス
    /// </summary>
    public class NormalEnemyBulletAI
    {
        public int AiGroupId {
            private set;
            get;
        }

        public List<EnemyBulletBase.EnemyBulletGroup> BulletPattern {   //!< 弾丸パターン
            private set;
            get;
        }

        public float Speed {    //!< 移動速度
            private set;
            get;
        }

        public float ShotInterval { //!< 発射間隔
            private set;
            get;
        }

        public int MovePattern {    //!< 移動パターン
            private set;
            get;
        }

        public int TransId {        //!< 向きID
            private set;
            get;
        }

        public float LeapTime {      //!< 徐々に向き直るフラグ
            private set;
            get;
        }

        public float SwitchTime {   //!< 切り替え時間
            private set;
            get;
        }

        public int BezId {          //!< ベジェ曲線ID
            private set;
            get;
        }

        public NormalEnemyBulletAI(int i_groupId, int i_pattern, float i_spd, float i_interval, int i_move, int i_trans, float i_leap_time, float i_time, int i_bez_id) {
            AiGroupId = i_groupId;
            BulletPattern = EnemyBulletBase.getBulletGroupList(i_pattern);
            Speed = i_spd;
            ShotInterval = i_interval;
            TransId = i_trans;
            LeapTime = i_leap_time;
            MovePattern = i_move;
            SwitchTime = i_time;
            BezId = i_bez_id;
        }
    }

    public static List<string> getNormalFormationLineStrDataListFromResorcesPath(string path) {
        TextAsset normal = Resources.Load<TextAsset>(path);
        return CommonUtil.InitLineData(normal.text.Split(new char[] { '\n', '\r' }).ToList<string>());
    }

    public static List<string> getNormalFormationLineStrDataListFromStrData(string data) {
        return CommonUtil.InitLineData(data.Split(new char[] { '\n', '\r' }).ToList<string>());
    }

    public static void setNormalFormationData(ref Dictionary<int, EnemyFormation> formData, ref int max_id, List<string> data) {
        int cnt = 0;
        foreach (string line in data) {
            string[] enemy = line.Split(new char[] { ',' });
            CommonUtil.Unset(ref enemy);
            List<NormalEnemyData.NormalEnemyBulletAI> enemyAi = new List<NormalEnemyBulletAI>();
            foreach (var aiData in EnemyFactory.normalEnemyAiData.Where(ai => ai.Value.AiGroupId == int.Parse(enemy[8]))) {
                enemyAi.Add(aiData.Value);
            }
            formData.Add(cnt, new EnemyFormation(int.Parse(enemy[0]), int.Parse(enemy[1]), float.Parse(enemy[2]), float.Parse(enemy[3]), float.Parse(enemy[4]), int.Parse(enemy[5]), int.Parse(enemy[6]), enemy[7], enemyAi));
            max_id = int.Parse(enemy[0]);
            cnt++;
        }
    }
}
