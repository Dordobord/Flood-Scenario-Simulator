using System;
using System.Collections.Generic;
using UnityEngine;

namespace StormWaits
{
    [Serializable]
    public class SelectChoice
    {
        public string label;
        public bool isCorrect;
        [Tooltip("Optional. Used by Drag To Bag and Grid Pack. Without a sprite the item is a plain square with its name.")]
        public Sprite icon;
        [Header("Grid Pack only")]
        [Tooltip("How many grid cells wide this piece is before rotating.")]
        [Min(1)] public int gridWidth = 1;
        [Tooltip("How many grid cells tall this piece is before rotating.")]
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
        [Tooltip("How many bar lengths the marker travels per second.")]
        public float cursorSpeed = 1f;
        [Range(0.05f, 0.5f)] public float targetWidth = 0.2f;
        [Min(1)] public int requiredHits = 1;

        [Header("Drag To Bag")]
        [Tooltip("Wrong drops allowed before the attempt fails. 0 means no limit.")]
        [Min(0)] public int maxMistakes = 0;

        [Header("Choices (Select Correct: 7 or fewer. Drag To Bag: 12 or fewer.)")]
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
        [Tooltip("Percent of households doing this in the HHI survey (reference only).")]
        public float hhiPercent;
        public bool isRequired = true;

        [Header("Reward and minigame")]
        [Tooltip("Optional. Leave empty if the task gives no item.")]
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
}
