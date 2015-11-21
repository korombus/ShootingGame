using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBulletBase : BulletBase {

    public List<GameObject> bulletList = new List<GameObject>();    //!< 弾丸リスト

    public override void Start() {
        enable = true;
        damage = 5f;
        speed = 10f;
    }

	void Update () {
        if (YkSys.Pose) {
            if (!base.CheckBulletEnable(bulletList)) {
                this.gameObject.SetActive(false);
            }
        }
	}

    /// <summary>
    /// 弾丸をスタートさせる
    /// </summary>
    public void StartBullet() {
        base.StartBullet(ref bulletList);
    }

    public void SetStopBullet() {
        if (!base.CheckBulletEnable(bulletList)) {
            this.gameObject.SetActive(false);
        }
    }
}
