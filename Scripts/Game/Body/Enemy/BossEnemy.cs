using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BossEnemy : CharBase {

    [SerializeField]
    GameObject ExplosionPrefab;     //!< 爆発プレハブ

    [SerializeField]
    GameObject[] WallObjects;

    // 外部変数
    public GameObject EnemyBulletBaseObject;    //!< 敵弾丸統括オブジェクト

    // 内部変数
    private int movePattern;                //!< 移動パターン
    private List<EnemyBulletBase.EnemyBulletGroup> bulletPattern; //!< 弾丸パターン
    private Animator anim;                  //!< 自身のアニメータ
    private float flashTimer = 0f;          //!< 明滅時間
    private float shotTimer = 0f;           //!< 次弾発射時間
    private float shotInterval = 0f;        //!< 弾丸撃ちだし時間
    private bool shotEnable = true;         //!< 弾の撃ちだしフラグ
    private bool beginRetreatFlag = false;  //!< 撤退開始
    private float moveTimer = 0f;
    private BossEnemyData.BossEnemyFormation bossData;
    private IEnumerable<NormalEnemyData.EnemyFormation> summonFormation;
    private int bossAIIndex = 0;            //!< ボスAIのインデックス
    private float AIChangeTimer = 0;        //!< ボスAI切り替えタイマー
    private float summonTimer = 0;
    private float summonInterval = 0;
    private Slider hpSlider;                //!< 体力用スライダー
    private EnemyFactory factory;


    public void SetData(BossEnemyData.BossEnemyFormation boss_data, GameObject baseObj, GameObject[] walls, EnemyFactory factoryClass) {
        EnemyBulletBaseObject   = baseObj;
        WallObjects             = walls;
        hp                      = boss_data.life;
        bossData                = boss_data;
        factory                 = factoryClass;
    }

    public override void Start() {
        base.Start();
        enable = true;
        flash = false;
        anim = this.GetComponent<Animator>();
        // 体力バーをセット
        hpSlider = parent.bossHpSlider;
        hpSlider.maxValue = hp;
        hpSlider.value = hp;
        hpSlider.gameObject.SetActive(true);

        // 初期化
        SetAI(bossData.boss_ai[0]);
    }

    void Update() {
        if (YkSys.Pose) {
            if (parent.player.activeSelf && !beginRetreatFlag) {
                // 移動
                if (moveTimer >= bossData.move_time) {
                    if (hp > 0) Move();
                }

                // 弾を撃つ
                if (shotTimer >= shotInterval) Shot();
                shotTimer += shotEnable ? Time.deltaTime : 0;

                // 通常敵を召喚
                if (summonTimer >= summonInterval) SummonFormation();
                summonTimer += Time.deltaTime;

                // AI切り替え
                if (AIChangeTimer >= bossData.boss_ai[bossAIIndex].switch_time) ChangeAI();
                AIChangeTimer += Time.deltaTime;

                // 撤退
                if (bossData.retreatFlag && this.hp < bossData.retreatLife && !beginRetreatFlag) {
                    BeginRetreat();
                }
            }
            else {
                Retreat();
            }
        }
        else {
            // 停止
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        // ダメージをくらっているならば明滅させる
        anim.SetBool("Damage", flash);
        if (flash) {
            flashTimer += Time.deltaTime;
            if (flashTimer >= 0.2f) {
                flash = false;
                flashTimer = 0f;
            }
        }

        // 動き出す時間まで待機
        if (moveTimer < bossData.move_time) { moveTimer += Time.deltaTime; }
    }

    void OnTriggerEnter2D(Collider2D col2) {
        // プレイヤーの弾に当たったらダメージ処理
        if (col2.tag == "PlayerBullet" && !beginRetreatFlag) {
            hp -= col2.transform.parent.gameObject.GetComponent<PlayerBulletBase>().damage;
            hpSlider.value = hp;
            flash = true;
        }

        // 体当たりの場合はダメージを与える
        if (col2.tag == "PlayerDamage") {
            col2.gameObject.GetComponent<PlayerDamage>().SetData(50);
        }
        // 死亡ラインに来たら初期位置に戻す
        if (col2.tag == "DeadLine") {
            if (!beginRetreatFlag) {
                Reposition();
            }
            else {
                // 勝利シナリオを再生
                factory.stopAllBullet();
                factory.nowCreateId = 0;
                factory.state = EnemyFactoryState.normal;
                parent.win = true;
                if (parent.GetType().Equals(typeof(BattleDebug))) {
                    SetStopKeyAsBattleDebug();
                    parent.player.GetComponent<DebugPlayer>().Score = this.score;
                }
                else {
                    SetScenarioState();
                    parent.player.GetComponent<Player>().Score = this.score;
                }
                Destroy(this.gameObject);
            }
        }

        // ボスは体力がなくなった場合にのみ削除
        if (hp <= 0f) {
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

            // 体力ゲージを消す
            hpSlider.gameObject.SetActive(false);

            // 勝利シナリオを再生
            factory.stopAllBullet();
            factory.nowCreateId = 0;
            factory.state = EnemyFactoryState.normal;
            parent.win = true;
            if (parent.GetType().Equals(typeof(BattleDebug))) {
                SetStopKeyAsBattleDebug();
                parent.player.GetComponent<DebugPlayer>().Score = this.score;
            }
            else {
                SetScenarioState();
                parent.player.GetComponent<Player>().Score = this.score;
            }
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// 弾を撃つ
    /// </summary>
    void Shot() {
        EnemyBulletBaseObject.GetComponent<EnemyBulletBase>().SetData(bulletPattern, this.transform);
        shotTimer = 0f;
    }

    /// <summary>
    /// 移動制御
    /// </summary>
    void Move() {
        this.GetComponent<Rigidbody2D>().velocity = EnemyMoveDirection[movePattern] * speed;
    }

    /// <summary>
    /// 通常敵を召喚
    /// </summary>
    void SummonFormation() {
        foreach (var enemy in summonFormation) {
            factory.CreateNormalEnemy(enemy);
        }
        summonTimer = 0;
    }

    /// <summary>
    /// ボスのAIを切り替える
    /// </summary>
    void ChangeAI() {
        bossAIIndex += 1;
        // 一周したら最初に戻す
        if (bossData.boss_ai.Count <= bossAIIndex) bossAIIndex = 0;

        // 切り替えタイミングを初期化
        AIChangeTimer = 0;

        // 召喚タイマーを初期化
        summonTimer = 0;

        // ここでデータを切り替える
        SetAI(bossData.boss_ai[bossAIIndex]);
    }

    public void BeginRetreat() {
        hpSlider.gameObject.SetActive(false);
        if (parent.GetType().Equals(typeof(BattleDebug))) {
            parent.player.GetComponent<DebugPlayer>().enable = false;
        }
        else {
            parent.player.GetComponent<Player>().enable = false;
        }
        beginRetreatFlag = true;
    }

    /// <summary>
    /// AIをセット
    /// </summary>
    /// <param name="data">ボスAIデータ</param>
    void SetAI(BossEnemyData.BossAI data) {
        movePattern     = data.movePattern;
        bulletPattern   = data.bulletType;
        shotInterval    = data.bulletShotInterval;
        summonInterval  = data.summon_interval;
        summonFormation = factory.getNormalEnemyData().Values.Where(normal => normal.id == data.summon_enemy_form_id);
        Rotation(data.trans_id, data.leap_time);
        speed           = data.speed;
    }


    void Rotation(int transId, float rotateLeapTime) {
        Quaternion quat = Quaternion.Euler(0, 0, 0);
        switch ((Direction)transId) {
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

        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, quat, rotateLeapTime);
    }

    /// <summary>
    /// ボスの位置を直す
    /// </summary>
    void Reposition() {
        switch ((WallType)bossData.trans_id) {
            case WallType.UP:
                this.transform.position = new Vector3(bossData.pos, WallObjects[bossData.trans_id].transform.position.y, 0);
                break;

            case WallType.DOWN:
                this.transform.position = new Vector3(bossData.pos, WallObjects[bossData.trans_id].transform.position.y, 0);
                this.transform.Rotate(0, 0, 180);
                break;

            case WallType.RIGHT:
                this.transform.position = new Vector3(WallObjects[bossData.trans_id].transform.position.x, bossData.pos, 0);
                this.transform.Rotate(0, 0, -90);
                break;

            case WallType.LEFT:
                this.transform.position = new Vector3(WallObjects[bossData.trans_id].transform.position.x, bossData.pos, 0);
                this.transform.Rotate(0, 0, 90);
                break;
        }
    }

    void Retreat() {
        switch ((WallType)bossData.trans_id) {
            case WallType.UP:
                this.GetComponent<Rigidbody2D>().velocity = EnemyMoveDirection[1] * 3;
                break;

            case WallType.DOWN:
                this.GetComponent<Rigidbody2D>().velocity = EnemyMoveDirection[0] * 3;
                break;

            case WallType.RIGHT:
                this.GetComponent<Rigidbody2D>().velocity = EnemyMoveDirection[3] * 3;
                break;

            case WallType.LEFT:
                this.GetComponent<Rigidbody2D>().velocity = EnemyMoveDirection[2] * 3;
                break;
        }
    }
}
