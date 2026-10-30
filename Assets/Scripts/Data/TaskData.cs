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

    [Header("Tuning")]
    [TextArea] public string revealMessage;
    public bool showRainfallWarning;

    [Header("Choices")]
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
    public TaskPriority priority = TaskPriority.Important;
    public ScoreMethod scoreMethod = ScoreMethod.Done;
    [TextArea] public string recommendation;

    [Header("Reward and minigame")]
    public ItemData rewardItem;
    public MinigameSettings minigame = new MinigameSettings();

    [Header("Placing")]
    public bool mustBePlaced;

    [Header("Power")]
    public bool needsPower;

    [Header("After completing")]
    public bool reopenToRead;
}

[Serializable]
public struct TaskResult
{
    public TaskData task;
    public int attempts;
    public int mistakes;
    public float secondsSpent;
}
