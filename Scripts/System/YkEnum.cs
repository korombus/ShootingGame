/// <summary>
/// 演算子
/// </summary>
public enum Operator
{
    NONE,   // 不正
    PLUS,   // 足し算
    MINE,   // 引き算
    MULT,   // 掛け算
    DIVI,   // 割り算
}

/// <summary>
/// ステージの状態
/// </summary>
public enum Stage
{
    none = 0,   // 不正データ
    title,      // タイトル画面
    opening,    // オープニング
    stage1,     // ステージ１
    stage2,     // ステージ２
    stage3,     // ステージ３
    stage4,     // ステージ４
    stage5,     // ステージ５
    stage6,     // ステージ６
    stageEx,    // ステージEx
}

/// <summary>
/// 敵の生成状態
/// </summary>
public enum EnemyFactoryState
{
    none,   //!< 生成しない
    normal, //!< ノーマル
    boss,   //!< ボス
}

/// <summary>
/// 敵撃ち方パターン
/// </summary>
public enum EnemyBulletPattern
{
    NONE = 0,                   // 撃たない
    ONE_SHOT,                   // 一発撃ち
    THREE_WAYS,                 // 三発拡散撃ち
    ONE_TIME_STOP_SHOT,         // 一時停止一発撃ち
    THREE_WAYS_TIME_STOP_SHOT,  // 一時停止三発拡散撃ち
    HOMING,                     // 自機狙い撃ち
    CIRCLEA,                    // 円状撃ち
    ONE_TIME_STOP_CIRCLEA,      // 円状停止撃ち
    INVER_HOMING,               // 自機以外狙い撃ち
}

/// <summary>
/// 敵の移動パターン
/// </summary>
public enum EnemyMovePattern
{
    UP_FROM_DOWN = 0, // 上から下へ
    DOWN_FROM_UP,     // 下から上へ
    RIGHT_FROM_LEFT,  // 右から左へ
    LEFT_FROM_RIGHT,  // 左から右へ
    UP_SLANT_RIGHT,   // 上から斜め右へ
    UP_SLANT_LEFT,    // 上から斜め左へ
    DOWN_SLANT_RIGHT, // 下から斜め右へ
    DOWN_SLANT_LEFT,  // 下から斜め左へ
}

/// <summary>
/// 移動効果
/// </summary>
public enum Direction
{
    NONE = -1,  // 無し
    UP,         // 上向き
    DOWN,       // 下向き
    RIGHT,      // 右向き
    LEFT,       // 左向き
}

/// <summary>
/// SEのタイプ
/// </summary>
public enum SoundEffectType
{
    NONE = 0,       // 無し
    SETUP_BUTTON,   // 強化ボタン
    TITLE_BUTTON,   // タイトルボタン
    BOOM,           // 爆発
    PLAYER_SHOOT,   // プレイヤー弾丸
    PLAYER_LASER,   // プレイヤーレーザー
    ENEMY_SHOOT,    // 敵弾丸
}

/// <summary>
/// シナリオパターンタイプ
/// </summary>
public enum ScenarioPatternType
{
    NONE = -1,  // 不正
    OPENING,    // オープニング台詞
    START,      // スタート台詞
    BOSS,       // ボス台詞
    WIN,        // ボス勝利後台詞
}

/// <summary>
/// シナリオ読み出しの状態
/// </summary>
public enum StateReadScenario
{
    NONE,   // 何もなし
    BEGINE, // 始まり
    READ,   // 読み出し中
    STOP,   // 停止
    END,    // 終了
    WAIT,   // 一時停止
}

/// <summary>
/// シナリオのコマンドタイプ
/// </summary>
public enum ScenarioCommandType
{
    NONE,       // 不正
    SHARP,      // #
    LINECOM,    // []内コマンド
}

public enum ScenarioSharpCommand
{
    NONE,       // 不正
    DISP,       // 画像表示
    DISPCLEAR,  // 画像非表示
    BGM,        // BGM再生
    SE,         // SE再生
    BGIMG,      // 背景画像設定
    FADEBGM,    // フェードBGM専用コマンド
    WAIT,       // ウェイトコマンド
}

/// <summary>
/// プレイヤーの強化タイプ
/// </summary>
public enum PlayerEnhanceType
{
    NONE,           // 不正
    LIFE,           // 耐久
    SPEED,          // 移動速度
    SHOT_SPEED,     // 発射速度
    CHARGE_SPEED,   // チャージ速度
}

/// <summary>
/// 壁タイプ
/// </summary>
public enum WallType
{
    UP = 0,
    DOWN,
    RIGHT,
    LEFT,
}

/// <summary>
/// プレイヤーのゲージタイプ
/// </summary>
public enum GaugeType
{
    NONE = 0,   // 不正
    BULLET,     // 弾ゲージ
    OPTION,     // オプションゲージ
    BOMB,       // 爆弾ゲージ
}

public enum SoundPatternCriAtom
{
    BGM,
    AMBIENT,
    EFFECT,
    MUSICAL,
    UI,
}

public enum StageBackgroundType
{
    FRONT,
    MIDDLE,
    SEMI_MIDDLE,
    BACK,
}

public enum NumberType
{
    INT,
    FLOAT,
    DOUBLE,
    LONG,
}