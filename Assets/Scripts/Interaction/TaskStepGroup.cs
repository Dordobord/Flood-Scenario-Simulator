using UnityEngine;

public class TaskStepGroup : MonoBehaviour
{
    [Header("Task")]
    [SerializeField] private TaskData task;

    private int stepCount;
    private int doneCount;
    private float firstStepTime;

    public bool IsTaskDone
    {
        get { return task == null || (TaskManager.Instance != null && TaskManager.Instance.IsCompleted(task)); }
    }

    private void Start()
    {
        if (stepCount == 0) Debug.LogWarning("TaskStepGroup: " + name + " has no steps under it.");
    }

    public void Register()
    {
        stepCount++;
    }

    public void CompleteStep(string progressFormat, string stepName)
    {
        if (IsTaskDone) return;

        if (doneCount == 0) firstStepTime = Time.time;
        doneCount++;

        if (doneCount < stepCount)
        {
            GameEvents.RaiseMessage(string.Format(progressFormat, stepName, doneCount, stepCount));
            return;
        }

        if (TaskManager.Instance != null) TaskManager.Instance.RecordAttempt(task, true, Time.time - firstStepTime);
    }

    public void UndoStep()
    {
        if (!IsTaskDone && doneCount > 0) doneCount--;
    }
}
