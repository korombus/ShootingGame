using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MoveHelp : MonoBehaviour {

    private Func<bool> m_func;
    private bool init = true;
    private Vector3 StartEndPos = new Vector3(0, 0, 0);
    private Vector3 HelpEndPos = new Vector3(-1100, 0, 0);

    public void SetData(Func<bool> func) {
        m_func = func;
        this.gameObject.SetActive(true);
    }

    void Update() {
        if (GetStartGameFlag() && Input.GetMouseButtonDown(0)) {
            m_func();
            init = false;
        }

        if (init) {
            this.gameObject.transform.localPosition = Vector3.Slerp(this.gameObject.transform.localPosition, StartEndPos, 0.3f);
        }
        else {
            this.gameObject.transform.localPosition = Vector3.Slerp(this.gameObject.transform.localPosition, HelpEndPos, 0.3f);
            if (GetEndPosFlag()) this.gameObject.SetActive(false);
        }
    }

    private bool GetStartGameFlag() {
        return this.transform.localPosition.x <= 1 && init;
    }

    private bool GetEndPosFlag() {
        return this.transform.localPosition.x <= -1000;
    }
}
