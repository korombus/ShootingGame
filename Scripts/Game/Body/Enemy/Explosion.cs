using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    public RuntimeAnimatorController controller;

    bool start = false;
    float timer = 0f;

    public void SetData(){
        start = true;
        timer = 0f;
        this.GetComponent<Animator>().runtimeAnimatorController = controller;
        this.gameObject.SetActive(true);
    }

    void Update() {
        if (start) {
            if (timer >= 1f) {
                this.gameObject.SetActive(false);
                start = false;
            }
            timer += Time.deltaTime;
        }
    }
}
