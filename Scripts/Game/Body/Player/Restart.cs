using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Restart : MonoBehaviour {

    // 外部変数
    public RuntimeAnimatorController controller;
    public Animator anim;
    public Text restartText;
    public List<GameObject> buttons = new List<GameObject>();

    // 内部編成
    private float restartCount = YkSys.CONTINUE_NUM;

    public void UpRestart() {
        anim.runtimeAnimatorController = controller;
        restartText.text = "Continue? (あと" + restartCount + "回)";
        if (restartCount > 0) {
            buttons.ForEach(act => act.SetActive(true));
        }
        else {
            buttons[1].SetActive(true);
        }
        this.gameObject.SetActive(true);
    }

    public void OnClickRetryButton() {
        CommonSys.GetSystem<TopWindow>().OnClickRestartButton();
        restartCount -= 1;
        buttons.ForEach(act => act.SetActive(false));
        this.gameObject.SetActive(false);
    }

    public void OnClickReturnButton() {
        Application.LoadLevel(Data.d_StageNameList[1]);
    }
}
