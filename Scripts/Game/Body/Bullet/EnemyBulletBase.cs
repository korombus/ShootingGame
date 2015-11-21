using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyBulletBase : BulletBase {

    [SerializeField]
    GameObject EnemyBulletPrefab;   //!< 弾プレハブ

    public class EnemyBulletGroup
    {
        private int _groupId;
        private int _bulletRadDataId;
        private int _damage;
        private float _speed;
        private float _moveStartTime;
        private float _stopTime;
        private float _stopEndTime;
        private bool _leapStopFlag;
        private bool _homing;
        private GameObject _bulletPrefab;

        public int GroupId { get { return _groupId; } }
        public int BulletRadDataId { get { return _bulletRadDataId; } }
        public int Damage { get { return _damage; } }
        public float Speed { get { return _speed; } }
        public float MoveStartTime { get { return _moveStartTime; } }
        public float StopTime { get { return _stopTime; } set { _stopTime = 0; } }
        public float StopEndTime { get { return _stopEndTime; } }
        public bool LeapStopFlag { get { return _leapStopFlag; } }
        public bool Homing { get { return _homing; } }
        public GameObject BulletPrefab { get { return _bulletPrefab; } }

        public EnemyBulletGroup(int i_groupId, int i_radId, int i_damage, float i_speed, float i_start, float i_stopTime, float i_stopEndTime, bool i_leap, bool i_homing, string i_prefab) {
            this._groupId = i_groupId;
            this._bulletRadDataId = i_radId;
            this._damage = i_damage;
            this._speed = i_speed;
            this._moveStartTime = i_start;
            this._stopTime = i_stopTime;
            this._stopEndTime = i_stopEndTime;
            this._leapStopFlag = i_leap;
            this._homing = i_homing;
            this._bulletPrefab = Resources.Load<GameObject>("Prefabs/Bullet/" + i_prefab);
        }
    }

    public class EnemyBulletRad
    {
        private Vector2 _radian;
        private float _randRadian;

        public Vector2 Radius { get { return _radian; } }
        public float RandRadius { get { return _randRadian; } }

        public EnemyBulletRad(float i_radian, float i_rand) {
            this._radian = new Vector2(Mathf.Cos(i_radian * Mathf.Deg2Rad), Mathf.Sin(i_radian * Mathf.Deg2Rad));
            this._randRadian = i_rand;
        }
    }

    // 内部変数
    private List<GameObject> bulletList = new List<GameObject>();

    public override void Start() {
        for (int i = 0; i < 50; i++) {
            GameObject obj = CommonUtil.PrefabInstance(EnemyBulletPrefab, this.transform);
            if (obj != null) {
                bulletList.Add(obj);
                obj.SetActive(false);
            }
        }
        enable = true;
    }

    /// <summary>
    /// SetData
    /// </summary>
    /// <param name="pattern">弾丸生成パターン(int)</param>
    /// <param name="trans">生成位置</param>
    public void SetData(List<EnemyBulletGroup> pattern, Transform trans) {
        pattern.ForEach(bulletData =>
        {
            GameObject obj = CreateBullet(trans, bulletData.Damage, bulletData.Speed);
            if (obj != null) {
                obj.GetComponent<EnemyBullet>().SetData(bulletData);
            }
        });
    }

    /// <summary>
    /// 弾丸作成
    /// </summary>
    /// <returns>弾丸オブジェクト</returns>
    private GameObject CreateBullet(Transform trans, int damageVal, float speedVal) {
        // 停止しているオブジェクトを探す
        GameObject obj = GetUnenableObject(bulletList);

        // なければ生成する
        if (obj == null) {
            obj = CommonUtil.PrefabInstance(EnemyBulletPrefab, this.transform);
            if (obj != null) {
                bulletList.Add(obj);
            }
        }

        // 弾丸の情報を設置
        obj.transform.position = trans.position;
        obj.SetActive(true);
        return obj;
    }
}
