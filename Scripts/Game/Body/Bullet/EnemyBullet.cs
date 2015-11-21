using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class EnemyBullet : BulletBase {

    public float SPEED {
        private set;
        get;
    }

    public float DAMAGE {
        private set;
        get;
    }

    // 内部変数
    private Vector2 distance = new Vector2(0, -1).normalized;   //!< 弾丸進行方向
    private EnemyBulletBase.EnemyBulletGroup myData;
    private bool stopFlag = false;  //!< 停止弾丸フラグ
    private bool isStop = true;    //!< 停止したかどうかのフラグ
    private float timer = 0f;       //!< タイマー
    private float startTime = 0f;   //!< 移動開始時間
    private bool homing;            //!< 自機狙いフラグ

    /// <summary>
    /// SetData
    /// </summary>
    /// <param name="i_distance">二次元座標</param>
    /// <param name="stop">停止フラグ</param>
    public void SetData(EnemyBulletBase.EnemyBulletGroup info){
        base.Start();
        myData = info;
        
        SPEED = info.Speed;
        DAMAGE = info.Damage;

        if (info.Homing) {
            distance = (parent.player.transform.position - this.transform.position).normalized;
        }
        else {
            float randRad = 1f;
            if (EnemyBulletBase.EnemyBulletRadData[info.BulletRadDataId].RandRadius > 0) {
                randRad = Random.Range(-EnemyBulletBase.EnemyBulletRadData[info.BulletRadDataId].RandRadius, EnemyBulletBase.EnemyBulletRadData[info.BulletRadDataId].RandRadius);
            }
            distance = EnemyBulletBase.EnemyBulletRadData[info.BulletRadDataId].Radius * randRad;
        }
        isStop = myData.StopTime <= 0f;
    }

    void Update() {
        if (YkSys.Pose && parent.player.activeSelf ) {
            if (myData.MoveStartTime <= startTime) {
                this.GetComponent<Rigidbody2D>().velocity = distance.normalized * GetBulletSpeedValue();
                if (!isStop) {
                    BulletStop();
                    timer += Time.deltaTime;
                }
            }
        }
        else {
            // 停止
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        startTime += Time.deltaTime;

        // 30秒以上生きていることはあんまりあり得ないのであり得たら止める
        /* 2015/10/20 ひとまず必要がなくなったのでコメントアウト
        if (startTime >= 30f) {
            startTime = 0f;
            this.gameObject.SetActive(false);
        }
        */
    }

    void OnTriggerEnter2D(Collider2D col2) {
        // プレイヤーならばダメージを与えて停止
        if (col2.gameObject.tag == "PlayerDamage") {
            col2.gameObject.GetComponent<PlayerDamage>().SetData(this.DAMAGE);
            Dead();
        }

        // プレイヤーか壁に当たった時点で停止
        if (col2.gameObject.tag == "Wall") {
            Dead();
        }

        if (col2.tag == "PlayerLaser") {
            Dead();
        }
    }

    /// <summary>
    /// 弾を止める
    /// </summary>
    /// <returns>弾の状態</returns>
    private void BulletStop(){
        if (timer >= myData.StopTime && !stopFlag) {
            stopFlag = true;
            timer = 0f;
        }

        if (timer >= myData.StopEndTime && stopFlag) {
            stopFlag = false;
            isStop = true;
            timer = 0f;
        }
    }

    /// <summary>
    /// 弾丸の速度を取得する
    /// </summary>
    /// <returns>弾速</returns>
    private float GetBulletSpeedValue() {
        if (stopFlag) {
            if (myData.LeapStopFlag && this.SPEED > 0) {
                this.SPEED -= 0.02f;
            }
            else {
                this.SPEED = 0f;
            }
        }
        else {
            this.SPEED = myData.Speed;
        }
        return this.SPEED;
    }

    private void Dead(){
        startTime = 0f;
        this.gameObject.SetActive(false);
    }
}
