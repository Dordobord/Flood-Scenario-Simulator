using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SelectChoice
{
    public string label;
    public bool isCorrect;
    public Sprite icon;
    [Header("Go bag only")]
    [Min(1)] public int gridWidth = 1;
    [Min(1)] public int gridHeight = 1;
}

[Serializable]
public class MinigameSettings
{
    public MinigameType type = MinigameType.HoldProgress;
    [TextArea] public string prompt;

    [Header("Hold Progress")]
    public float holdSeconds = 3f;

    [Header("Timing Bar")]
    public float cursorSpeed = 1f;
    [Range(0.05f, 0.5f)] public float targetWidth = 0.2f;
    [Min(1)] public int requiredHits = 1;

    [Header("Tuning")]
    [TextArea] public string revealMessage;

    [Header("Choices (Select Correct: 7 or fewer. Go bag: any number.)")]
    public List<SelectChoice> choices = new List<SelectChoice>();
}

[CreateAssetMenu(fileName = "NewTask", menuName = "Storm Waits/Task")]
public class TaskData : ScriptableObject
{
    public string id;
    public string title = "New Task";
    [TextArea] public string description;

    [Header("Preparedness")]
    public PreparednessDomain domain;
    [Range(0f, 100f)]
    public float hhiPercent;
    public bool isRequired = true;
    [TextArea] public string recommendation;

    [Header("Reward and minigame")]
    public ItemData rewardItem;
    public MinigameSettings minigame = new MinigameSettings();
}

[Serializable]
public struct TaskResult
{
    public TaskData task;
    public int attempts;
    public int mistakes;
    public float secondsSpent;
}
