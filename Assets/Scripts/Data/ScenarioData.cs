using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScenario", menuName = "Storm Waits/Scenario")]
public class ScenarioData : ScriptableObject
{
    [Header("Info")]
    public ScenarioType type = ScenarioType.Urban;
    public string displayName = "New Scenario";
    [TextArea(3, 8)] public string description;

    [Header("Scene")]
    public string sceneName;

    [Header("Preparation")]
    public float preparationSeconds = 180f;

    [Header("Tasks")]
    public List<TaskData> tasks = new List<TaskData>();
}
