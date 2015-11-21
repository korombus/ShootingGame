using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : CharBase {

    [SerializeField]
    GameObject bulletPrefab;        //!< 弾プレハブ  

    [SerializeField]
    GameObject playerOptionPrefab;        //!< オプション機体

    [SerializeField]
    float setBulletPosY = 0.33f;    //!< 弾丸初期位置

    [SerializeField]
    PlayerLaser playerLaser;

    [SerializeField]
    bool muteki = false;


    private const string UI_FULL_HP = "ui_stage_16";
    private const string UI_CAUTION_HP = "ui_stage_18";
    private const string UI_DANGER_HP = "ui_stage_17";

    // 外部プロパティ
    public float Score {
        set {
            this.score += value;
            if (value > 0) { parent.SetTotalScore(value); }
        }
        get { return this.score; }
    }

    /// <summary>
    /// プレイヤー強化パラメータ
    /// </summary>
    public class PlayerEnhanceParam
    {
        public float playerEnhanceLifePt;       //!< 耐久度
        public float playerEnhanceSpeedPt;      //!< 移動速度
        public float playerEnhanceShotSpeedPt;  //!< 連射速度
        public float playerEnhanceChargeSpeedPt; //!< チャージ速度
        public PlayerEnhanceParam(float val) {
            playerEnhanceLifePt         = val;
            playerEnhanceSpeedPt        = val;
            playerEnhanceShotSpeedPt    = val;
            playerEnhanceChargeSpeedPt  = val;
        }
    }

    // 外部変数
    public GameObject optionParent;     //!< オプションの親

    // 内部変数
    private float timer = 0f;
    private float maxHp = 0f;
    private float beforeScore = 0f;
    private float optionGaugeDownVal = 0.8f;                            //!< オプションゲージを下げる値
    private List<GameObject> bullet = new List<GameObject>();           //!< 弾リスト
    private List<GameObject> playerOption = new List<GameObject>();     //!< オプションリスト

    public override void Start() {
        base.Start();
        enable = true;
        // デバッグビルドの際にだけ入れる
        if (Debug.isDebugBuild) {
            muteki = YkSys.mutekiFlag;
        }
        hp = YkSys.playerParam.playerEnhanceLifePt > 0 ? YkSys.playerParam.playerEnhanceLifePt * 20 : 20;
        maxHp = hp;
        speed = 0.98f * (YkSys.playerParam.playerEnhanceSpeedPt + 1);
        SetHpData(hp);

        flash = false;
        parent.hpSlider.maxValue = maxHp;
        parent.hpSlider.value = maxHp;

        score = parent.TotalScore;

        // オプションを回転させる為のオブジェクトを取得
        if (optionParent == null) optionParent = CommonUtil.SearchObjectChild("PlayerOption", this.transform);

        // ゲージの最大値を設置
        // 弾ゲージ
        parent.bulletEnergyGauge.maxValue = 100 + ((YkSys.playerParam.playerEnhanceChargeSpeedPt > 0 ? (int)YkSys.playerParam.playerEnhanceChargeSpeedPt : 1) * 10);
        bulletGauge = parent.bulletEnergyGauge.value = parent.bulletEnergyGauge.maxValue;

        // オプションゲージ
        parent.PlayerOptGauge.maxValue = 100 + ((YkSys.playerParam.playerEnhanceShotSpeedPt > 0 ? (int)YkSys.playerParam.playerEnhanceShotSpeedPt : 1) * 10);
        OptionGauge = parent.PlayerOptGauge.value = parent.PlayerOptGauge.maxValue;
        bombGauge = 100 + ((YkSys.playerParam.playerEnhanceShotSpeedPt > 0 ? (int)YkSys.playerParam.playerEnhanceShotSpeedPt : 1) * 10);

        // 弾丸を一部作成
        for (int i = 0; i < 10; i++) {
            CreateBullet(false);
        }
    }

    void Update() {
        if (YkSys.Pose && this.enable) {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");
            Vector2 direction = new Vector2(x, y).normalized;
            GetComponent<Rigidbody2D>().velocity = direction * speed;

            // 弾撃ち
            if (Input.GetKey(KeyCode.C) && timer >= (1.0f / (YkSys.playerParam.playerEnhanceShotSpeedPt > 0 ? YkSys.playerParam.playerEnhanceShotSpeedPt : 1))) {
                Shot();
                timer = 0;
            }
            timer += Time.deltaTime;

            // レーザー撃ち
            if (Input.GetKeyDown(KeyCode.D) && bulletGauge >= parent.bulletEnergyGauge.maxValue) {
                ShotLaser();
            }

            // オプション射出
            SetUpOption(Input.GetKey(KeyCode.X));
        } else {
            // 停止
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }

        // スコアはドラムロール式で増やす
        if (beforeScore < this.score) {
            beforeScore += Mathf.RoundToInt(beforeScore / this.score);
            parent.SetScoreText(beforeScore);
        }

        // 弾エネルギーを回復
        ChangeGaugeVal(Operator.PLUS);

        // オプションを回転
        RotOption();
    }

    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="i_dama">ダメージ量</param>
    public void Damage(float i_dama) {
        if (!muteki) {
            hp -= i_dama;
            parent.hpSlider.value = hp;
            SetHpData(hp);

            // 体力が無くなったら停止
            if (hp <= 0) {
                parent.restartButton.GetComponent<Restart>().UpRestart();
                this.gameObject.SetActive(false);
            }
        }
    }

    void CreateBullet(bool bulletEnable = true) {
        GameObject obj = CommonUtil.PrefabInstance(bulletPrefab, this.transform);
        if (obj != null) {
            Vector3 pos = this.transform.position;
            pos.y += setBulletPosY;
            obj.transform.position = pos;
            parent.bgmSound.ChangeVolume(OPTION.SEVolume, true);
            bullet.Add(obj);
            obj.SetActive(bulletEnable);
        }
    }

    /// <summary>
    /// レーザーを撃つ
    /// </summary>
    void ShotLaser() {
        parent.bgmSound.CueSheetNameSE = Data.d_CriCueSheetNameTable[SoundPatternCriAtom.EFFECT];
        parent.bgmSound.Play(Data.d_SoundEffect[SoundEffectType.PLAYER_LASER], true);

        if (playerLaser.transform.parent != this.transform) {
            playerLaser.transform.localPosition = this.transform.localPosition;
            playerLaser.transform.parent = this.transform;
        }

        playerLaser.OnPlayLaser();

        // 弾エネルギーを消費
        ChangeGaugeVal(Operator.MINE, GaugeType.BULLET);
    }

    /// <summary>
    /// 弾丸を撃つ
    /// </summary>
    void Shot() {
        // 全ての弾がアクティブであれば新しい弾を作成する
        if (CheckActiveBullet()) {
            CreateBullet();
        }
        else {
            AwakeBullet();
        }

        parent.bgmSound.CueSheetNameSE = Data.d_CriCueSheetNameTable[SoundPatternCriAtom.EFFECT];
        parent.bgmSound.Play(Data.d_SoundEffect[SoundEffectType.PLAYER_SHOOT], true);
    }

    /// <summary>
    /// 弾エネルギーゲージを変更
    /// </summary>
    /// <param name="ope">演算子</param>
    void ChangeGaugeVal(Operator ope, GaugeType type = GaugeType.NONE){
        // 無敵状態ならば各種ゲージを減らないようにする
        if (muteki) return;

        switch (ope) {
            case Operator.PLUS:
                // 弾ゲージ
                if (bulletGauge < parent.bulletEnergyGauge.maxValue) {
                    bulletGauge += ((YkSys.playerParam.playerEnhanceChargeSpeedPt > 0 ? YkSys.playerParam.playerEnhanceChargeSpeedPt : 1) / 10);
                }

                // オプションゲージ
                if (OptionGauge < parent.PlayerOptGauge.maxValue) {
                    OptionGauge += ((YkSys.playerParam.playerEnhanceChargeSpeedPt > 0 ? YkSys.playerParam.playerEnhanceChargeSpeedPt : 1) / 30);
                }
                break;
            case Operator.MINE:
                // 弾ゲージ
                if (bulletGauge > 0 && type == GaugeType.BULLET) {
                    bulletGauge = 0;
                }

                // オプションゲージ
                if (OptionGauge > 0 && type == GaugeType.OPTION) {
                    OptionGauge -= optionGaugeDownVal;
                }
                break;
        }

        // 各種ゲージを増減
        parent.bulletEnergyGauge.value  = bulletGauge;
        parent.PlayerOptGauge.value     = OptionGauge;
    }


    /// <summary>
    /// 弾のアクティブを調べる
    /// </summary>
    /// <returns>bool</returns>
    private bool CheckActiveBullet() {
        if (bullet.Count <= 0) return true;
        bool active = bullet.FindAll(act => act.activeSelf == true).Count == bullet.Count;
        return active;
    }

    /// <summary>
    /// 弾丸を起こす
    /// </summary>
    void AwakeBullet() {
        GameObject obj = bullet.Find(act => act.activeSelf == false);
        Vector3 pos = this.transform.position;
        pos.y += setBulletPosY;
        obj.transform.position = pos;
        obj.GetComponent<PlayerBulletBase>().StartBullet();
        obj.SetActive(true);
    }

    /// <summary>
    /// プレイヤーオプションをセット
    /// </summary>
    void SetUpOption(bool input) {
        if (OptionGauge > optionGaugeDownVal && input) {
            if (playerOption.Count < 2) {
                // オプション作成
                GameObject obj = CommonUtil.PrefabInstance(playerOptionPrefab);
                if (obj != null) {
                    obj.GetComponent<PlayerOption>().SetData(CommonUtil.SearchObjectChild("OptionUnit_" + playerOption.Count, optionParent.transform));
                    playerOption.Add(obj);
                }
            }
            else {
                // 止まっているのがあれば起こす
                if (playerOption.Exists(opt => opt.activeSelf == false) && OptionGauge > optionGaugeDownVal * 1.8f)
                    playerOption.Find(opt => opt.activeSelf == false).SetActive(true);
            }
            // オプションゲージを減らす
            ChangeGaugeVal(Operator.MINE, GaugeType.OPTION);
        }
        else {
            // ゲージがなければオプションを出さない
            playerOption.ForEach(opt => opt.SetActive(false));
        }
    }

    /// <summary>
    /// リスタートする
    /// </summary>
    public void Restart() {
        if (hp > 0) return;
        hp = maxHp;
        parent.hpSlider.value = hp;
        SetHpData(hp);
        parent.restartButton.SetActive(false);
        this.gameObject.SetActive(true);
    }

    void SetHpData(float i_hp) {
        float damageScore = Mathf.Round((i_hp / maxHp) * 100);
        parent.damagePerText.text = damageScore <= 100 ? damageScore < 0 ? "0" : damageScore.ToString() : "100";
        SetHpSprite(GetHpSpriteName());
    }

    void SetHpSprite(string i_sp) {
        parent.damagePerText.transform.parent.GetComponent<Image>().sprite = parent.GetStageUISprites(i_sp);
    }

    private string GetHpSpriteName() {
        float damageScore = Mathf.Round((hp / maxHp) * 100);
        if (damageScore > 60) {
            return UI_FULL_HP;
        }
        else if (damageScore > 20 && damageScore <= 60) {
            return UI_CAUTION_HP;
        }
        else {
            return UI_DANGER_HP;
        }
    }

    /// <summary>
    /// オプションを回転させる
    /// </summary>
    void RotOption() {
        optionParent.transform.Rotate(0, 0, +((YkSys.playerParam.playerEnhanceSpeedPt + 1) / 2.7f), Space.Self);
    }
}
