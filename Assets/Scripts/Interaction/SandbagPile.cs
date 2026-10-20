using UnityEngine;

public class SandbagPile : MonoBehaviour, IInteractable
{
    [Header("Task")]
    [SerializeField] private TaskData task;
    [SerializeField] private SandbagWall wall;

    public string Prompt
    {
        get { return GameSettings.Current.sandbagTakePrompt; }
    }

    public bool CanInteract
    {
        get
        {
            if (task == null || task.rewardItem == null) return false;
            if (TaskManager.Instance != null && TaskManager.Instance.IsCompleted(task)) return false;
            if (wall != null && wall.IsFull) return false;
            return true;
        }
    }

    public void Interact()
    {
        if (!CanInteract) return;

        GameSettings settings = GameSettings.Current;
        InventoryManager inventory = InventoryManager.Instance;
        if (inventory == null) return;

        if (inventory.Contains(task.rewardItem))
        {
            GameEvents.RaiseMessage(settings.sandbagAlreadyCarryingMessage);
            return;
        }

        if (!inventory.TryAdd(task.rewardItem))
        {
            GameEvents.RaiseMessage(settings.inventoryFullMessage);
            return;
        }

        GameEvents.RaiseMessage(settings.collectedMessage + task.rewardItem.displayName);
    }
}
