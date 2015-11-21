using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharBase : PrimBase {

    // 敵の移動方向テーブル
    public Dictionary<int, Vector2> EnemyMoveDirection = new Dictionary<int, Vector2>()
    {
        {0, new Vector2(0f, -1f).normalized},
        {1, new Vector2(0f, 1f).normalized},
        {2, new Vector2(-1f, 0f).normalized},
        {3, new Vector2(1f, 0f).normalized},
        {4, new Vector2(0.5f, -0.5f).normalized},
        {5, new Vector2(-0.5f, -0.5f).normalized},
        {6, new Vector2(0.5f, 0.5f).normalized},
        {7, new Vector2(-0.5f, 0.5f).normalized},
        {8, new Vector2(0.1f, 0.1f).normalized},
        {9, new Vector2(-0.5f, 0.5f).normalized},
    };

    /**************************************************
    * 定数
    **************************************************/
    public const int MOVE_PATTERN_NUM = 8;  //!< 移動タイプ数
    public const int MOVE_EFFECT_NUM = 4;   //!< 移動効果数

   /**************************************************
    * 共通で必要な情報
    **************************************************/
    public bool enable; //!< 挙動許可


   /**************************************************
    * プレイ画面で必要な情報
    **************************************************/
    protected float hp;         //!< 体力
    protected float speed;      //!< 移動速度
    protected float score;      //!< 点数

    protected float bulletGauge;  //!< 弾丸ゲージ
    protected float OptionGauge;  //!< オプションゲージ
    protected float bombGauge;    //!< ボムゲージ

    protected bool flash;       //!< 明滅判定

    protected void SetStopKeyAsBattleDebug() {
        CommonSys.GetSystem<BattleDebug>().stopEscKey = true;
    }

    protected void SetScenarioState() {
        CommonSys.GetSystem<TopWindow>().scenarioState = ScenarioPatternType.WIN;
    }

}
