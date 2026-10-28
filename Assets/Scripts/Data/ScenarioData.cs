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

    [Header("Evacuation")]
    public string evacuationCenterName = "Evacuation Center";

    [Header("Preparation")]
    public float preparationSeconds = 180f;

    [Header("Weather")]
    [Range(0f, 1f)] public float prepRainStrength = 0.5f;
    [Range(0f, 1f)] public float evacuationRainStrength = 0.8f;
    public RainfallWarningLevel rainfallWarning = RainfallWarningLevel.Orange;

    [Header("Water")]
    public Material floodWaterMaterial;

    [Header("Tasks")]
    public List<TaskData> tasks = new List<TaskData>();
}
