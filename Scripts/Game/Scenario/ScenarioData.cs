using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScenarioData {
    private ScenarioPatternType _scenarioType;  //!< シナリオのタイプ
    private Stage _stageState;                  //!< ステージの状態
    private TextAsset _scenario;                //!< シナリオ本文
    public bool IsRead;                         //!< 既に読まれているかどうか

    public ScenarioPatternType ScenarioType { get { return _scenarioType; } }
    public Stage StageState     { get { return _stageState; } }
    public TextAsset Scenario   { get { return _scenario; } }

    public ScenarioData(ScenarioPatternType i_type, Stage i_state, TextAsset i_scenario, bool i_read) {
        _scenarioType   = i_type;
        _stageState     = i_state;
        _scenario       = i_scenario;
        IsRead          = i_read;
    }
}

public class ScenarioDebugData
{
    private ScenarioPatternType _scenarioType;  //!< シナリオのタイプ
    private Stage _stageState;                  //!< ステージの状態
    private string _scenario;                //!< シナリオ本文
    public bool IsRead;                         //!< 既に読まれているかどうか

    public ScenarioPatternType ScenarioType { get { return _scenarioType; } }
    public Stage StageState     { get { return _stageState; } }
    public string Scenario   { get { return _scenario; } }

    public ScenarioDebugData(ScenarioPatternType i_type, Stage i_state, string i_scenario, bool i_read) {
        _scenarioType   = i_type;
        _stageState     = i_state;
        _scenario       = i_scenario;
        IsRead          = i_read;
    }
}
