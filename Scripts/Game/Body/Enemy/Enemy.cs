using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Debug = UnityEngine.Debug;

public class Enemy : CharBase {

    [SerializeField]
    GameObject ExplosionPrefab;     //!< 爆発プレハブ

    // 外部変数
    public GameObject EnemyBulletBaseObject;    //!< 敵弾丸統括オブジェクト
    public SpriteRenderer renderEnable { private set; get; }
    public RuntimeAnimatorController animatorController;
    public bool enemy2 = false;

    // 内部変数
    private EnemyFactory factory;
    private List<EnemyBulletBase.EnemyBulletGroup> bulletPattern;   //!< 弾丸パターン
    private List<NormalEnemyData.NormalEnemyBulletAI> enemyAi;
    private float flashTimer = 0f;          //!< 明滅時間
    private float shotTimer = 0f;           //!< 次弾発射時間
    private float bezWarmUpTimer = 0f;
    private float aiTimer = 0f;
    private bool shotEnable = true;         //!< 弾の撃ちだしフラグ
    private bool colorChange = false;
    public bool circle = false;
    private int wallCnt = 0;
    private int aiIndex = 0;
    private Bezier bezier;

    public void SetData(NormalEnemyData.EnemyFormation enemyFormData, GameObject baseObj, EnemyFactory factory) {
        EnemyBulletBaseObject   = baseObj;
        hp                      = enemyFormData.life;
        score                   = enemyFormData.score;
        enemyAi                 = enemyFormData.enemyAI;
        this.factory            = factory;
        SetBezierState();
    }

	public override void Start () {
        base.Start();
        enable = true;
        flash = false;
        renderEnable = this.GetComponent<SpriteRenderer>();
        renderEnable.enabled = false;
        this.gameObject.AddComponent<Animator>().runtimeAnimatorController = animatorController;
	}


    void Update() {
        if (YkSys.Pose && parent.player.activeSelf) {
            // 体力があれば動く
            if (hp > 0) Move();
            // 弾を撃つ
            if (shotTimer >= enemyAi[aiIndex].ShotInterval) Shot();
            shotTimer += shotEnable ? Time.deltaTime : 0;
            
            // AI切り替え
            if (aiTimer >= enemyAi[aiIndex].SwitchTime) {
                SetEnemyAi();
                //Move();
                aiTimer = 0;
            }
            aiTimer += Time.deltaTime;
        } else {
            // 停止
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        // ダメージをくらっているならば明滅させる
        if (flash) {
            Damage();
        }
    }

    void OnTriggerStay2D(Collider2D col2) {
        if (col2.tag == "PlayerLaser") {
            hp -= col2.gameObject.GetComponent<PlayerLaser>().damage;
            if (hp <= 0f) {
                Explosion();
                SetScore();
                Death();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col2) {
        // プレイヤーの弾に当たったらダメージ処理
        if (col2.tag == "PlayerBullet") {
            hp -= col2.transform.parent.gameObject.GetComponent<PlayerBulletBase>().damage;
            flash = true;
        }

        // プレイヤーに接触した場合と体力がない場合は削除
        if (col2.tag == "PlayerDamage" || col2.tag == "PlayerOption" || hp <= 0f) {
            Explosion();
            // 体当たりの場合はダメージを与える
            if (col2.tag == "PlayerDamage") {
                col2.gameObject.GetComponent<PlayerDamage>().SetData(10);
            }
            // スコアを加算する
            SetScore();
            Death();
        }

        // 死亡線に来たら削除する
        if (col2.tag == "DeadLine") {
            Death();
        }

        // 壁に当たったらそれ以上弾を撃たない
        if (col2.tag == "Wall") {
            wallCnt++;
            if (wallCnt == 1) renderEnable.enabled = false; 
            if (wallCnt >= 2) shotEnable = false;
        }

        if (col2.tag == "BackGround") {
            renderEnable.enabled = true;
        }
    }

    /// <summary>
    /// 弾を撃つ
    /// </summary>
    void Shot() {
        EnemyBulletBaseObject.GetComponent<EnemyBulletBase>().SetData(enemyAi[aiIndex].BulletPattern, this.transform);
        shotTimer = 0f;
    }

    /// <summary>
    /// 移動制御
    /// </summary>
    bool bezInit = true;
    float bezTimer = 0f;
    void Move() {
        if (enemy2) {
            if (enemyAi[aiIndex].MovePattern == 2 || enemyAi[aiIndex].MovePattern == 5 || enemyAi[aiIndex].MovePattern == 7) {
                this.gameObject.GetComponent<Animator>().SetBool("Right", true);
            }
            else if (enemyAi[aiIndex].MovePattern == 3 || enemyAi[aiIndex].MovePattern == 4 || enemyAi[aiIndex].MovePattern == 6) {
                this.gameObject.GetComponent<Animator>().SetBool("Left", true);
            }
            else {
                this.gameObject.GetComponent<Animator>().SetBool("Right", false);
                this.gameObject.GetComponent<Animator>().SetBool("Left", false);
            }
        }

        if (circle) {
            Vector3 bezPos = bezier.GetPointAtTime(bezTimer);
            this.transform.position = new Vector3(bezPos.x, bezPos.y, 0);
            if (bezTimer <= DirectionData.bezierDirectionData[enemyAi[aiIndex].BezId].bezTime) {
                bezTimer += 0.001f;
            }
            else {
                circle = false;
            }
        }
        else {
            GetComponent<Rigidbody2D>().velocity = EnemyMoveDirection[enemyAi[aiIndex].MovePattern] * enemyAi[aiIndex].Speed;
            if (bezInit) {
                if (bezWarmUpTimer >= DirectionData.bezierDirectionData[enemyAi[aiIndex].BezId].bezWarmTime) {
                    DirectionData.BezierBase bezData = DirectionData.bezierDirectionData[enemyAi[aiIndex].BezId];
                    bezier = new Bezier(this.transform.localPosition, bezData.CtlPoint1, bezData.CtlPoint2, bezData.endPoint);
                    circle = true;
                }
                bezWarmUpTimer += Time.deltaTime;
            }
        }
    }

    void Rotation() {
        Quaternion quat = Quaternion.Euler(0, 0, 0);
        switch ((Direction)enemyAi[aiIndex].TransId) {
            case Direction.UP:
                quat = Quaternion.Euler(0, 0, -180);
                break;
            case Direction.RIGHT:
                quat = Quaternion.Euler(0, 0, 90);
                break;
            case Direction.LEFT:
                quat = Quaternion.Euler(0, 0, -90);
                break;
        }
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, quat, enemyAi[aiIndex].LeapTime);
    }

    void SetEnemyAi() {
        aiIndex++;
        if (enemyAi.Count <= aiIndex) {
            aiIndex = 0;
        }
        SetBezierState();
        Rotation();
    }

    void SetBezierState() {
        if (enemyAi[aiIndex].BezId >= 0) {
            bezWarmUpTimer = 0f;
            bezInit = true;
        }
        else {
            bezInit = false;
        }
    }

    void SetScore() {
        if (parent.GetType().Equals(typeof(BattleDebug))) {
            parent.player.GetComponent<DebugPlayer>().Score = this.score;
        }
        else {
            parent.player.GetComponent<Player>().Score = this.score;
        }
    }

    void Explosion() {
        GameObject obj = factory.getExplosionObject();
        if (obj == null) {
            obj = CommonUtil.PrefabInstance(ExplosionPrefab);
            factory.setExplosionObject(obj);
        }
        if (obj != null) {
            obj.transform.position = this.transform.position;
            obj.GetComponent<Explosion>().SetData();
            parent.bgmSound.CueSheetNameSE = Data.d_CriCueSheetNameTable[SoundPatternCriAtom.EFFECT];
            parent.bgmSound.ChangeVolume(OPTION.SEVolume, true);
            parent.bgmSound.Play(Data.d_SoundEffect[SoundEffectType.BOOM], true);
        }
    }

    void Damage() {
        Color col = renderEnable.color;
        if (!colorChange) {
            col.r -= 0.2f;
        }
        else {
            col.r += 0.2f;
        }
        renderEnable.color = col;
        if (col.r <= 0) {
            colorChange = true;
        }
        else if (col.r >= 1) {
            colorChange = false;
        }

        flashTimer += Time.deltaTime;
        if (flashTimer >= 0.2f) {
            flash = false;
            colorChange = false;
            col.r = 1;
            renderEnable.color = col;
            flashTimer = 0f;
        }
    }

    void Death() {
        shotTimer = 0;
        aiIndex = 0;
        aiTimer = 0;
        wallCnt = 0;
        this.gameObject.SetActive(false);
    }
}
