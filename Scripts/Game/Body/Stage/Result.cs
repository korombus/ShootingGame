using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Result : MonoBehaviour {

    // 定数
    const float BONUS_NUM = 10000;
    const string INIT_BONUS_TEXT = "10000 x ";

    // 外部変数
    public Text m_ScoreText;    //!< スコア表示テキスト
    public List<Text> m_ResultBonusTextList = new List<Text>();  //!< 結果表示用テキストリスト
    public Text m_DefficultRankBonusText;   //!< 難易度ボーナステキスト

    // 内部変数
    private bool scoreRoll = true;      //!< スコアをロールさせるフラグ
    private float defBonusScore = 0f;   //!< 難易度ボーナススコア
    private float playerScore = 0f;     //!< プレイヤーが獲得したスコア
    private float resultScore = 0f;     //!< 結果スコア

    /// <summary>
    /// SetData
    /// </summary>
    /// <param name="score">獲得スコア</param>
	public void SetData(float score){
        
        // 各ボーナスの状態を表示
        m_ResultBonusTextList[0].text = INIT_BONUS_TEXT + YkSys.playerParam.playerEnhanceLifePt * 2;
        m_ResultBonusTextList[1].text = INIT_BONUS_TEXT + YkSys.playerParam.playerEnhanceSpeedPt;
        m_ResultBonusTextList[2].text = INIT_BONUS_TEXT + YkSys.playerParam.playerEnhanceShotSpeedPt;
        m_ResultBonusTextList[3].text = INIT_BONUS_TEXT + YkSys.playerParam.playerEnhanceChargeSpeedPt;

        // 現在のスコアを表示
        m_ScoreText.text = score.ToString();
        playerScore = score;

        // ボーナスのスコアを計算
        defBonusScore = (BONUS_NUM + YkSys.playerParam.playerEnhanceLifePt * 2) + 
                        (BONUS_NUM + YkSys.playerParam.playerEnhanceSpeedPt) + 
                        (BONUS_NUM + YkSys.playerParam.playerEnhanceShotSpeedPt) + 
                        (BONUS_NUM + YkSys.playerParam.playerEnhanceChargeSpeedPt);
        
        // 難易度別のボーナスを表示
        m_DefficultRankBonusText.text = defBonusScore.ToString();

        // 今回ステージのスコアを保持
        resultScore = score + defBonusScore;
    }

    void Update() {
        // 始めのアニメーションが終わったらボーナス値をスコアに追加
        if (this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("End")) {
            // 結果のスコアよりプレイヤースコアのが小さければ値を変化させる
            if (resultScore > playerScore && scoreRoll) {
                playerScore += Mathf.Floor(resultScore / 30.0f);
                defBonusScore -= Mathf.Floor(resultScore / 30.0f);
                // 値を表示
                m_ScoreText.text = playerScore.ToString();
                m_DefficultRankBonusText.text = defBonusScore.ToString();
            }
            // 結果の値を越していたら是正する
            if (resultScore <= playerScore) {
                scoreRoll = false;
                m_ScoreText.text = resultScore.ToString();
                m_DefficultRankBonusText.text = "0";
            }
        }

        // スコアロールが終わったら次のステージへ
        if (!scoreRoll && Input.GetMouseButtonDown(0)) {
            if (Application.loadedLevelName != BattleDebug.BUTTLE_DEBUG_SCENE_NAME) {
                // 現在が最後のステージだったらタイトルへ戻る
                if (YkSys.nowStage == (CommonSys.GetSystem() as TopWindow).GetStringToStageEnum("stage" + YkSys.STAGE_NUM)) {
                    Application.LoadLevel(0);
                }
                else {
                    // 継続するのであればステージを再読み込み
                    Application.LoadLevel(1);
                }
            }
            else {
                // デバッグであればデバッグ終了
                (CommonSys.GetSystem() as BattleDebug).debugStart = false;
                this.gameObject.SetActive(false);
            }
        }
    }
}
