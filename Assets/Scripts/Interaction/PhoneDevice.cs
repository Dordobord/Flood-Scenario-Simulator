using UnityEngine;

public class PhoneDevice : MonoBehaviour
{
    [Header("Phone")]
    [SerializeField] private ItemData phoneItem;

    private void Start()
    {
        if (InventoryManager.Instance != null) InventoryManager.Instance.SelectionChanged += OnSelectionChanged;
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null) InventoryManager.Instance.SelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        bool phoneSelected = phoneItem != null && InventoryManager.Instance.SelectedItem == phoneItem;

        if (!phoneSelected)
        {
            if (MinigameManager.IsPhoneOpen) MinigameManager.Instance.ClosePhone();
            return;
        }

        if (MinigameManager.IsPhoneOpen)
        {
            MinigameManager.Instance.ClosePhone();
            return;
        }

        if (CanOpen()) MinigameManager.Instance.BeginPhone(this);
    }

    private bool CanOpen()
    {
        if (MinigameManager.Instance == null || MinigameManager.IsActive) return false;
        return PhaseManager.Instance != null && PhaseManager.Instance.CurrentPhase == GamePhase.Preparation;
    }

    public void OnAppFinished(TaskData task, bool success)
    {
        if (!success || task == null) return;

        InventoryManager inventory = InventoryManager.Instance;

        if (task.rewardItem != null && inventory != null && !inventory.Contains(task.rewardItem) && inventory.TryAdd(task.rewardItem))
        {
            GameEvents.RaiseMessage(string.Format(GameSettings.Current.collectedMessageFormat, task.rewardItem.displayName));
        }
    }
}
