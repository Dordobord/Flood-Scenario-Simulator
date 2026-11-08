using UnityEngine;

public class HazardMapAlert : MonoBehaviour
{
    [Header("Task")]
    [SerializeField] private TaskData hazardMapTask;

    private void Start()
    {
        PhoneAlertCard.IsPending = false;
        Invoke(nameof(ShowAlert), GameSettings.Current.hazardAlertDelay);
    }

    private void ShowAlert()
    {
        TaskManager tasks = TaskManager.Instance;
        if (tasks == null || hazardMapTask == null) return;
        if (!tasks.ActiveTasks.Contains(hazardMapTask) || tasks.IsCompleted(hazardMapTask)) return;
        if (PhaseManager.Instance != null && PhaseManager.Instance.CurrentPhase != GamePhase.Preparation) return;

        GameSettings settings = GameSettings.Current;

        PhoneAlertCard.IsPending = true;
        AudioManager.PlaySfx(SoundLibrary.Current.evacSiren);
        if (HudUI.Instance != null) HudUI.Instance.BlinkItem(settings.phoneAlertItem);
        GameEvents.RaiseMessage(settings.phoneAlertToast);
    }
}
