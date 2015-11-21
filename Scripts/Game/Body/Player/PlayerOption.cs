using UnityEngine;
using System.Collections;

public class PlayerOption : MonoBehaviour {

    // 定数
    int GUARD_NUM_MAX = 3;      //!< ガード回数最大値

    // 外部変数
    public GameObject optUnit;  //!< 追いかけるオブジェクト

    // 内部変数
    private int guardNum;       //!< ガード出来る回数


    public void SetData(GameObject unit) {
        optUnit = unit;
        guardNum = GUARD_NUM_MAX;
    }

    void Update() {
        this.transform.position = Vector3.Slerp(this.transform.position, optUnit.transform.position, 0.07f);
    }

    void OnTriggerEnter2D(Collider2D col2) {
        if (col2.tag == "EnemyBullet") {
            col2.gameObject.SetActive(false);
            guardNum -= 1;
        }

        if (col2.tag == "Enemy") {
            guardNum -= 1;
        }

        if (col2.tag == "Boss") {
            guardNum = 0;
        }

        // カウントが0以下であれば停止する
        if (guardNum <= 0) {
            this.gameObject.SetActive(false);
            guardNum = GUARD_NUM_MAX;
        }
    }
}
