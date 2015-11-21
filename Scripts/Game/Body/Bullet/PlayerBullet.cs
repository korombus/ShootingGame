using UnityEngine;
using System;
using System.Collections;

public class PlayerBullet : MonoBehaviour {

    private Func<bool> m_stopFunc;
    void Update() {
        if (YkSys.Pose) {
            GetComponent<Rigidbody2D>().velocity = transform.up.normalized * this.transform.parent.GetComponent<PlayerBulletBase>().speed;
        }
        else {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    void OnTriggerEnter2D(Collider2D col2) {
        if (col2.gameObject.tag == "Enemy" || col2.gameObject.tag == "Wall" || col2.gameObject.tag == "Boss") {
            this.gameObject.SetActive(false);
            this.transform.localPosition = this.transform.parent.localPosition;
            this.transform.parent.GetComponent<PlayerBulletBase>().SetStopBullet();
        }
    }
}
