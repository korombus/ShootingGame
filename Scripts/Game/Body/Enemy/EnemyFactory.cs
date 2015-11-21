using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EnemyFactory : MonoBehaviour {

    [SerializeField]
    GameObject ExplosionPrefab;     //!< 爆発プレハブ

    [SerializeField]
    GameObject EnemyPrefab;     //!< 敵プレハブ

    [SerializeField]
    GameObject[] WallObjects;   //!< 壁から生成する

    public static Dictionary<int, NormalEnemyData.NormalEnemyBulletAI> normalEnemyAiData = new Dictionary<int, NormalEnemyData.NormalEnemyBulletAI>();    //!< 通常敵のAIデータ

    // 外部変数
    public GameObject EnemyBulletBaseObject;    //!< 敵の弾丸生成オブジェクト
    public Animator BossCautionAnim;
    public bool initFlag = false;

    // 内部変数
    private float timer = 0f;
    private int maxId = 0;
    public EnemyFactoryState state = EnemyFactoryState.none;
    private Dictionary<int, NormalEnemyData.EnemyFormation> normalFormData = new Dictionary<int, NormalEnemyData.EnemyFormation>();
    private Dictionary<Stage, BossEnemyData.BossEnemyFormation> bossFormData = new Dictionary<Stage, BossEnemyData.BossEnemyFormation>();
    private Dictionary<string, Dictionary<int, GameObject>> readyNormalEnemyObjectDic = new Dictionary<string, Dictionary<int, GameObject>>();
    private List<GameObject> explosionObjectList = new List<GameObject>();
    private GameObject bossObject = null;


    /// <summary>
    /// 通常敵のAIデータ作成
    /// </summary>
    public static void CreateNormalEnemyAiData(string aiData = null) {
        string aiRoot = "EnemyFormationData/NormalEnemyAIData";
        if (aiData == null) {
            if (normalEnemyAiData.Count > 0) return;
            TextAsset data = Resources.Load<TextAsset>(aiRoot);
            if (data != null) {
                List<string> lineData = CommonUtil.InitLineData(data.text.Split(new char[] { '\n', '\r' }).ToList<string>());
                foreach (string line in lineData) {
                    string[] ai = line.Split(new char[] { ',' });
                    CommonUtil.Unset(ref ai);
                    normalEnemyAiData.Add(int.Parse(ai[0]), new NormalEnemyData.NormalEnemyBulletAI(int.Parse(ai[1]), int.Parse(ai[2]), float.Parse(ai[3]), float.Parse(ai[4]), int.Parse(ai[5]), int.Parse(ai[6]), float.Parse(ai[7]), float.Parse(ai[8]), ai.Length >= 10 ? int.Parse(ai[9]) : -1));
                }
            }
        }
        else {
            normalEnemyAiData.Clear();
            List<string> lineData = CommonUtil.InitLineData(aiData.Split(new char[] { '\n', '\r' }).ToList<string>());
            foreach (string line in lineData) {
                string[] ai = line.Split(new char[] { ',' });
                CommonUtil.Unset(ref ai);
                normalEnemyAiData.Add(int.Parse(ai[0]), new NormalEnemyData.NormalEnemyBulletAI(int.Parse(ai[1]), int.Parse(ai[2]), float.Parse(ai[3]), float.Parse(ai[4]), int.Parse(ai[5]), int.Parse(ai[6]), float.Parse(ai[7]), float.Parse(ai[8]), ai.Length >= 10 ? int.Parse(ai[9]) : -1));
            }
        }
    }

    void Awake() {
        // バトルスキップの場合はデータを生成しない
        if (!YkSys.battleSkip && Application.loadedLevelName != BattleDebug.BUTTLE_DEBUG_SCENE_NAME) {
            init();
            initFlag = true;
        }
        BossCautionAnim.gameObject.SetActive(false);
    }

    public void init(string normalData = null, string bossStrData = null) {
        string normalEnemyFormPath = "EnemyFormationData/enemy_" + EnemyFactoryState.normal + "_" + YkSys.nowStage;
        string bossEnemyFormPath = "EnemyFormationData/enemy_" + EnemyFactoryState.boss;

        List<string> lineData = new List<string>();
        if (normalData == null) {
            lineData = NormalEnemyData.getNormalFormationLineStrDataListFromResorcesPath(normalEnemyFormPath); 
        }else{
            lineData = NormalEnemyData.getNormalFormationLineStrDataListFromStrData(normalData);
        }

        NormalEnemyData.setNormalFormationData(ref normalFormData, ref maxId, lineData);
        lineData.Clear();

        if (bossStrData == null) {
            BossEnemyData.setBossFormationData(ref bossFormData, BossEnemyData.getBossFormationLineStrDataListFromResorcesPath(bossEnemyFormPath));
        }
        else {
            BossEnemyData.setBossFormationDataForDebug(ref bossFormData, BossEnemyData.getBossFormationLineStrDataListFromStrData(bossStrData));
        }
    }

    void Start() {
        if (EnemyBulletBaseObject == null) 
            EnemyBulletBaseObject = CommonUtil.SearchObjectChild("EnemyBulletFactory", this.transform);
        state = EnemyFactoryState.normal;

        if (initFlag) {
            CreateInitObjects();
        }
    }

    public void CreateInitObjects() {
        foreach (NormalEnemyData.EnemyFormation data in normalFormData.Values) {
            GameObject obj = CommonUtil.PrefabInstance(data.prefab, WallObjects[data.trans_id].transform);
            obj.SetActive(false);
            if (readyNormalEnemyObjectDic.ContainsKey(data.prefab.name)) {
                readyNormalEnemyObjectDic[data.prefab.name].Add(readyNormalEnemyObjectDic[data.prefab.name].Count, obj);
            }
            else {
                readyNormalEnemyObjectDic.Add(data.prefab.name, new Dictionary<int, GameObject>() { { 0, obj } });
            }
        }

        BossEnemyData.BossEnemyFormation formData = bossFormData[YkSys.nowStage];
        bossObject = CommonUtil.PrefabInstance(formData.prefab, WallObjects[formData.trans_id].transform);
        bossObject.SetActive(false);

        for (int i = 0; i < 20; i++) {
            GameObject obj = CommonUtil.PrefabInstance(ExplosionPrefab);
            obj.SetActive(false);
            explosionObjectList.Add(obj);
        }
    }

    public int nowCreateId = 0;
    void Update() {
        // ポーズ中は敵の生成を行わない
        if (YkSys.Pose) {
            if (!YkSys.battleSkip) {
                switch (state) {
                    case EnemyFactoryState.normal:
                        CreateNormalEnemy();
                        break;
                    case EnemyFactoryState.boss:
                        CreateBossEnemy();
                        break;
                }
            }

            // 敵と弾丸が全て無くなったらボスフラグを立てる
            if (GetEnableEnemyAtouchInAllWall() && !GetEnableEnemyBullet() && nowCreateId >= maxId && state == EnemyFactoryState.normal && readyNormalEnemyObjectDic.Count > 0) {
                if (Application.loadedLevelName != BattleDebug.BUTTLE_DEBUG_SCENE_NAME) {
                    if (!BossCautionAnim.gameObject.activeSelf) {
                        BossCautionAnim.gameObject.SetActive(true);
                    }
                    else {
                        (CommonSys.GetSystem() as TopWindow).boss = true;
                        (CommonSys.GetSystem() as TopWindow).scenarioState = ScenarioPatternType.BOSS;
                        state = EnemyFactoryState.boss;
                    }
                }
                else {
                    (CommonSys.GetSystem() as BattleDebug).boss = true;
                    state = EnemyFactoryState.boss;
                }
            }
            timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// 生きている敵がいるかどうかを調べる
    /// </summary>
    /// <returns></returns>
    private bool GetEnableEnemyAtouchInAllWall() {
        if (readyNormalEnemyObjectDic.Count <= 0) return true;
        int cnt, high = 0;
        foreach (var i in readyNormalEnemyObjectDic.Values) {
            cnt = i.Values.Count(x => x.activeSelf == true);
            if (cnt > high) high = cnt;
        }
        return high == 0;
    }

    /// <summary>
    /// 生きている弾丸があるかどうかを調べる
    /// </summary>
    /// <returns></returns>
    private bool GetEnableEnemyBullet() {
        foreach (Transform trans in EnemyBulletBaseObject.transform) {
            if (trans.gameObject.activeSelf) return true;
        }
        return false;
    }

    public void stopAllBullet() {
        foreach (Transform trans in EnemyBulletBaseObject.transform) {
            trans.gameObject.SetActive(false);
        }
    }

    private List<NormalEnemyData.EnemyFormation> nowNormalEnemyFormation = new List<NormalEnemyData.EnemyFormation>();
    private int beforeId = -1;
    /// <summary>
    /// 通常敵作成
    /// </summary>
    private void CreateNormalEnemy() {
        if (!nowCreateId.Equals(beforeId)) {
            nowNormalEnemyFormation.Clear();
            foreach(NormalEnemyData.EnemyFormation enemyData in normalFormData.Values.Where(val => val.id == nowCreateId)){
                nowNormalEnemyFormation.Add(enemyData);
            }
            beforeId = nowCreateId;
        }
        CreateNormalEnemyPri();
    }

    /// <summary>
    /// 通常敵作成
    /// </summary>
    /// <param name="formation"></param>
    private void CreateNormalEnemyPri() {
        
        nowNormalEnemyFormation.ForEach(data =>
        {
            if (data.move_time < timer) {
                if (CreateNormalEnemy(data)) {
                    nowNormalEnemyFormation.Remove(data);
                }
            }
        });

        if (nowNormalEnemyFormation.Count <= 0) {
            nowCreateId++;
            timer = 0f;
        }
    }

    public bool CreateNormalEnemy(NormalEnemyData.EnemyFormation enemy) {
        GameObject obj = null;
        Dictionary<int, GameObject>.ValueCollection readyEnemyObjects = readyNormalEnemyObjectDic[enemy.prefab.name].Values;
        if (isNormalEnemyUnenable(readyEnemyObjects)) {
            obj = getNormalEnemyUnenableGObj(readyEnemyObjects);
            if (obj.GetComponent<Enemy>().renderEnable != null) obj.GetComponent<Enemy>().renderEnable.enabled = false;
            obj.SetActive(true);
        }

        if (obj != null) {
            float x = enemy.pos;
            float y = enemy.pos;
            float scale = enemy.scale;
            obj.transform.localScale = new Vector3(scale, scale, 1);

            switch ((WallType)enemy.trans_id) {
                case WallType.UP:
                    obj.transform.position = new Vector3(x, WallObjects[enemy.trans_id].transform.position.y + 1, 0);
                    break;

                case WallType.DOWN:
                    obj.transform.position = new Vector3(x, WallObjects[enemy.trans_id].transform.position.y, 0);
                    obj.transform.Rotate(0, 0, 180);
                    break;

                case WallType.RIGHT:
                    obj.transform.position = new Vector3(WallObjects[enemy.trans_id].transform.position.x, y, 0);
                    obj.transform.Rotate(0, 0, -90);
                    break;

                case WallType.LEFT:
                    obj.transform.position = new Vector3(WallObjects[enemy.trans_id].transform.position.x, y, 0);
                    obj.transform.Rotate(0, 0, 90);
                    break;
            }
            // 敵を生成
            obj.GetComponent<Enemy>().SetData(enemy, EnemyBulletBaseObject, this);
            return true;
        }
        return false;
    }

    private bool isNormalEnemyUnenable(Dictionary<int, GameObject>.ValueCollection data){
        return data.Any(enemy => enemy.activeSelf == false);
    }


    private GameObject getNormalEnemyUnenableGObj(Dictionary<int, GameObject>.ValueCollection data) {
        return data.FirstOrDefault(enemyObj => enemyObj.activeSelf == false);
    }

    /// <summary>
    /// 通常敵のデータを取得
    /// </summary>
    /// <returns></returns>
    public Dictionary<int, NormalEnemyData.EnemyFormation> getNormalEnemyData() {
        return normalFormData;
    }

    /// <summary>
    /// ボス作成
    /// </summary>
    private void CreateBossEnemy() {
        // 現在のステージのボス隊列データを読み込み
        BossEnemyData.BossEnemyFormation formData = bossFormData[YkSys.nowStage];
        GameObject obj = bossObject;
        if (obj == null) {
            obj = CommonUtil.PrefabInstance(formData.prefab, WallObjects[formData.trans_id].transform);
        }
        if (obj != null) {
            float x = formData.pos;
            float y = formData.pos;
            float scale = formData.scale;
            obj.transform.localScale = new Vector3(scale, scale, 1);

            switch ((WallType)formData.trans_id) {
                case WallType.UP:
                    obj.transform.position = new Vector3(x, WallObjects[formData.trans_id].transform.position.y + 1.5f, 0);
                    break;

                case WallType.DOWN:
                    obj.transform.position = new Vector3(x, WallObjects[formData.trans_id].transform.position.y, 0);
                    obj.transform.Rotate(0, 0, 180);
                    break;

                case WallType.RIGHT:
                    obj.transform.position = new Vector3(WallObjects[formData.trans_id].transform.position.x, y, 0);
                    obj.transform.Rotate(0, 0, -90);
                    break;

                case WallType.LEFT:
                    obj.transform.position = new Vector3(WallObjects[formData.trans_id].transform.position.x, y, 0);
                    obj.transform.Rotate(0, 0, 90);
                    break;
            }

            // ここまできたら生成フラグをなくす
            state = EnemyFactoryState.none;

            // ボスを生成
            obj.GetComponent<BossEnemy>().SetData(formData, EnemyBulletBaseObject, WallObjects, this);
            obj.SetActive(true);
            BossCautionAnim.gameObject.SetActive(false);
        }
    }

    public void setExplosionObject(GameObject obj) {
        explosionObjectList.Add(obj);
    }

    public GameObject getExplosionObject() {
        return explosionObjectList.FirstOrDefault(explosion => explosion.activeSelf == false);
    }

    public void Reset() {
        normalFormData = new Dictionary<int, NormalEnemyData.EnemyFormation>();
        bossFormData = new Dictionary<Stage, BossEnemyData.BossEnemyFormation>();
        normalEnemyAiData = new Dictionary<int, NormalEnemyData.NormalEnemyBulletAI>();
    }
}
