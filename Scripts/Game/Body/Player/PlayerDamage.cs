using UnityEngine;
using System.Collections;

public class PlayerDamage : MonoBehaviour
{
    public void SetData(float damage) {
        if (GetDebug()) {
            this.transform.parent.GetComponent<DebugPlayer>().Damage(damage);
        }
        else {
            this.transform.parent.GetComponent<Player>().Damage(damage);
        }
    }

    public void SetScore(float score) {
        if (GetDebug()) {
            this.transform.parent.GetComponent<DebugPlayer>().Score = score;
        }
        else {
            this.transform.parent.GetComponent<Player>().Score = score;
        }
    }

    private bool GetDebug() {
        return this.transform.parent.gameObject.name == "DebugPlayer";
    }
}
