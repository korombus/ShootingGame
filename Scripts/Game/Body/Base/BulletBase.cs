using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BulletBase : PrimBase{

    // 敵弾丸グループテーブル
    public static Dictionary<int, EnemyBulletBase.EnemyBulletGroup> EnemyBulletGroupData = new Dictionary<int, EnemyBulletBase.EnemyBulletGroup>();

    // 敵弾丸の角度テーブル
    public static Dictionary<int, EnemyBulletBase.EnemyBulletRad> EnemyBulletRadData = new Dictionary<int, EnemyBulletBase.EnemyBulletRad>();

    /**************************************************
    * 定数
    **************************************************/
    public const int ENEMY_BULLET_PATTERN_NUM = 7; //!< 弾の種類


    /**************************************************
    * 共通で必要な情報
    **************************************************/
    public bool enable;     //!< 挙動許可
    public float damage;    //!< ダメージ量
    public float speed;     //!< 移動速度

    /// <summary>
    /// 敵の弾丸角度データを作成
    /// </summary>
    public static void CreateEnemyBulletRadData(string bulletRD = null) {
        if (bulletRD == null) {
            if (EnemyBulletRadData.Count > 0) return;
            // csvデータを読み込み
            TextAsset radData = Resources.Load<TextAsset>("EnemyBulletData/EnemyBulletRadData");
            if (radData != null) {
                List<string> digList = CommonUtil.GetCsvTextToList(radData);
                // 弾丸角度データをテーブル化
                foreach (var data in digList) {
                    string[] rad = data.Split(new char[] { ',' });
                    EnemyBulletRadData.Add(int.Parse(rad[0]), new EnemyBulletBase.EnemyBulletRad(float.Parse(rad[1]), float.Parse(rad[2])));
                }
            }
        }
        else {
            EnemyBulletRadData.Clear();
            List<string> lineData = CommonUtil.InitLineData(bulletRD.Split(new char[] { '\n', '\r' }).ToList<string>());
            foreach (string line in lineData) {
                string[] rad = line.Split(new char[] { ',' });
                EnemyBulletRadData.Add(int.Parse(rad[0]), new EnemyBulletBase.EnemyBulletRad(float.Parse(rad[1]), float.Parse(rad[2])));
            }
        }
    }

    /// <summary>
    /// 敵の弾丸グループデータを作成
    /// </summary>
    public static void CreateEnemyBulletGroupData(string bulletGD = null) {
        if (bulletGD == null) {
            if (EnemyBulletGroupData.Count > 0) return;
            TextAsset groupData = Resources.Load<TextAsset>("EnemyBulletData/EnemyBulletGroupData");
            if (groupData != null) {
                List<string> groupList = CommonUtil.GetCsvTextToList(groupData);
                // 弾丸グループデータをテーブル化
                foreach (var data in groupList) {
                    string[] group = data.Split(new char[] { ',' });
                    EnemyBulletGroupData.Add(int.Parse(group[0]), new EnemyBulletBase.EnemyBulletGroup(int.Parse(group[1]), int.Parse(group[2]), int.Parse(group[3]), float.Parse(group[4]), float.Parse(group[5]), float.Parse(group[6]), float.Parse(group[7]), group[8] == "1", group[9] == "1", group[10]));
                }
            }
        }
        else {
            EnemyBulletGroupData.Clear();
            List<string> lineData = CommonUtil.InitLineData(bulletGD.Split(new char[] { '\n', '\r' }).ToList<string>());
            foreach (string line in lineData) {
                string[] group = line.Split(new char[] { ',' });
                EnemyBulletGroupData.Add(int.Parse(group[0]), new EnemyBulletBase.EnemyBulletGroup(int.Parse(group[1]), int.Parse(group[2]), int.Parse(group[3]), float.Parse(group[4]), float.Parse(group[5]), float.Parse(group[6]), float.Parse(group[7]), group[8] == "1", group[9] == "1", group[10]));
            }
        }
    }

    /// <summary>
    /// 弾丸グループIDから弾丸グループデータを取得
    /// </summary>
    /// <param name="groupId">弾丸グループID</param>
    /// <returns>List</returns>
    public static List<EnemyBulletBase.EnemyBulletGroup> getBulletGroupList(int groupId) {
        List<EnemyBulletBase.EnemyBulletGroup> groupData = new List<EnemyBulletBase.EnemyBulletGroup>();
        foreach (var bullet in EnemyBulletGroupData.Where(group => group.Value.GroupId == groupId)) {
            groupData.Add(bullet.Value);
        }
        return groupData;
    }

    /// <summary>
    /// 弾丸をスタートさせる
    /// </summary>
    protected void StartBullet(ref List<GameObject> bulletList) {
        bulletList.ForEach(obj => obj.SetActive(true));
    }

    /// <summary>
    /// 弾丸が生きているかを調べる
    /// </summary>
    /// <returns></returns>
    protected bool CheckBulletEnable(List<GameObject> bulletList) {
        return bulletList.Exists(obj => obj.activeSelf == true);
    }

    protected GameObject GetUnenableObject(List<GameObject> bulletList) {
        return bulletList.FirstOrDefault(obj => obj.activeSelf == false);
    }

    /// <summary>
    /// 止まってるオブジェクトを取得
    /// </summary>
    /// <returns></returns>
    protected GameObject GetUnenableObject(Transform trans) {
        GameObject unenableObject = null;
        foreach (Transform obj in trans) {
            if (!obj.gameObject.activeSelf) {
                unenableObject = obj.gameObject;
                break;
            }
        }
        return unenableObject;
    }
}
