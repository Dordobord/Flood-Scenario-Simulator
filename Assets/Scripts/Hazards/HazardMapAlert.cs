using UnityEngine;

public class HazardMapAlert : MonoBehaviour
{
    [Header("Task")]
    [SerializeField] private TaskData hazardMapTask;

    private void Start()
    {
        Invoke(nameof(ShowAlert), GameSettings.Current.hazardAlertDelay);
    }

    private void ShowAlert()
    {
        TaskManager tasks = TaskManager.Instance;
        if (tasks == null || hazardMapTask == null) return;
        if (!tasks.ActiveTasks.Contains(hazardMapTask) || tasks.IsCompleted(hazardMapTask)) return;
        if (PhaseManager.Instance != null && PhaseManager.Instance.CurrentPhase != GamePhase.Preparation) return;

        GameEvents.RaiseMessage(GameSettings.Current.hazardAlertMessage);
    }
}
