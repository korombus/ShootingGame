using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetScreenSize : MonoBehaviour {

    [SerializeField]
    List<GameObject> planes = new List<GameObject>();   // 両方のパネル

	void Awake () {
        if (planes.Count <= 0) {
            foreach (GameObject obj in this.transform) {
                planes.Add(obj);
            }
        }

        // 画面DPIから位置を計算
        planes.Find(obj => obj.name == "LeftPlane").transform.position = new Vector3(-9, 0f, -1f);
        planes.Find(obj => obj.name == "RightPlane").transform.position = new Vector3(9, 0f, -1f);
	}
	
    /*
	void Update () {
	}
    */
}
