using UnityEngine;
using System.Collections;

public class PlayerLaser : MonoBehaviour {

    private const float LIMIT_LASER_COL_SIZE_Y = 19;

    public BoxCollider2D laserCol;
    public float damage = 0.5f;

    [SerializeField]
    private ParticleSystem particleSys;

    private float sizeVal = 0.1f;
    private float offsetVal = 0.1f;

    void Start() {
        sizeVal = 2.2f / particleSys.startSpeed;
        offsetVal = sizeVal / 1.7f;
    }

    public void OnPlayLaser() {
        laserCol.enabled = true;
        laserCol.offset = new Vector2(0, offsetVal);
        laserCol.size = new Vector2(0.7f, sizeVal);
        particleSys.Play();
    }

    void Update() {
        if (particleSys.isPlaying) {
            laserCol.offset = new Vector2(0, laserCol.offset.y + offsetVal);
            if (laserCol.size.y < LIMIT_LASER_COL_SIZE_Y) {
                laserCol.size = new Vector2(0.7f, laserCol.size.y + sizeVal);
            }
            else {
                this.transform.parent = null;
            }
        }
        else {
            laserCol.enabled = false;
        }
    }
}
