using UnityEngine;
using System.Collections;

public class BackScroll : MonoBehaviour {

    const float MAX_TRANS_Y = 3;
    const float MIN_TRANS_Y = -1;

    public bool enable = true;
    public float speed { private set; get; }

    private float vec_y = 0f;
    private string myId = "0";
    private BackScrollBase parent;

    void Start() {
        parent = this.transform.parent.GetComponent<BackScrollBase>();
        myId = this.gameObject.name.Substring(this.gameObject.name.Length - 1, 1);
        vec_y = this.transform.localPosition.y;
        parent.SetId(SetTexture, SetSpeed, StageBackground.GetBgImageSettingListGroupByStageType()[parent.id].bgSetStopStartFlag && !myId.Equals("0") ? parent.id + 2 : parent.id + 1);
    }

    public bool SetTexture(Texture tex) {
        this.GetComponent<Renderer>().material.mainTexture = tex;
        return true;
    }

    public bool SetSpeed(float spd) {
        speed = spd;
        return true;
    }

    public void SetEnable(bool i_en) {
        enable = i_en;
    }

    void Update () {
        if (!parent.stopOpenFlag) {
            if (enable) {
                changePosition();
            }
            else {
                if (vec_y < MAX_TRANS_Y) {
                    changePosition();
                }
            }
        }
	}

    void changePosition() {
        vec_y -= speed;
        this.transform.localPosition = new Vector3(this.transform.localPosition.x, vec_y, this.transform.localPosition.z);
        if (vec_y <= MIN_TRANS_Y) {
            vec_y = MAX_TRANS_Y;
            parent.SetId(SetTexture, SetSpeed, StageBackground.GetBgImageSettingListGroupByStageType()[parent.id].bgSetStopStartFlag && !myId.Equals("0") ? parent.id + 2 : parent.id + 1);
        }
    }
}
