using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }
    public static ScenarioData ChosenScenario;

    [SerializeField] private ScenarioData scenario;

    private List<TaskData> activeTasks = new List<TaskData>();
    private Dictionary<TaskData, int> attempts = new Dictionary<TaskData, int>();
    private Dictionary<TaskData, int> mistakes = new Dictionary<TaskData, int>();
    private Dictionary<TaskData, float> secondsSpent = new Dictionary<TaskData, float>();
    private Dictionary<TaskData, TaskResult> completed = new Dictionary<TaskData, TaskResult>();

    public event Action Changed;
    public event Action<TaskData> TaskCompleted;

    public ScenarioData CurrentScenario { get; private set; }

    public List<TaskData> ActiveTasks
    {
        get { return activeTasks; }
    }

    public int TotalCount
    {
        get { return activeTasks.Count; }
    }

    public int CompletedCount
    {
        get
        {
            int count = 0;
            foreach (TaskData task in activeTasks)
            {
                if (completed.ContainsKey(task)) count++;
            }
            return count;
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetChosenScenario()
    {
        ChosenScenario = null;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ScenarioData toLoad = ChosenScenario != null ? ChosenScenario : scenario;
        if (toLoad != null) LoadScenario(toLoad);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void LoadScenario(ScenarioData scenarioData)
    {
        CurrentScenario = scenarioData;
        activeTasks.Clear();
        attempts.Clear();
        mistakes.Clear();
        secondsSpent.Clear();
        completed.Clear();

        if (scenarioData != null)
        {
            foreach (TaskData task in scenarioData.tasks)
            {
                if (task != null) activeTasks.Add(task);
            }
        }

        if (Changed != null) Changed();
    }

    public void ResetTask(TaskData task)
    {
        if (task == null || !completed.Remove(task)) return;
        if (Changed != null) Changed();
    }

    public bool IsCompleted(TaskData task)
    {
        return task != null && completed.ContainsKey(task);
    }

    public int GetAttempts(TaskData task)
    {
        if (task != null && attempts.ContainsKey(task)) return attempts[task];
        return 0;
    }

    public bool TryGetResult(TaskData task, out TaskResult result)
    {
        if (task != null && completed.TryGetValue(task, out result)) return true;

        result = new TaskResult();
        return false;
    }

    public List<TaskResult> GetResults()
    {
        return new List<TaskResult>(completed.Values);
    }

    public void RecordAttempt(TaskData task, bool success, float seconds, int mistakeCount = 0)
    {
        if (task == null || IsCompleted(task)) return;

        attempts[task] = GetAttempts(task) + 1;

        int totalMistakes = 0;
        if (mistakes.ContainsKey(task)) totalMistakes = mistakes[task];
        mistakes[task] = totalMistakes + mistakeCount;

        float total = 0f;
        if (secondsSpent.ContainsKey(task)) total = secondsSpent[task];
        secondsSpent[task] = total + seconds;

        if (!success) return;

        TaskResult result = new TaskResult();
        result.task = task;
        result.attempts = attempts[task];
        result.mistakes = mistakes[task];
        result.secondsSpent = secondsSpent[task];

        completed[task] = result;
        if (TaskCompleted != null) TaskCompleted(task);
        if (Changed != null) Changed();
    }
}
