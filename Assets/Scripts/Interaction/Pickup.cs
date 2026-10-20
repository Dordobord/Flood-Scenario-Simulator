using UnityEngine;

public class Pickup : HoldInteractable, IInteractable
{
    [SerializeField] private TaskData task;
    [SerializeField] private string promptOverride;
    [SerializeField] private bool hideWhenCompleted = true;

    public string Prompt
    {
        get
        {
            if (task != null && task.needsPower && !PowerSupply.HasPower) return GameSettings.Current.noPowerPrompt;
            if (!string.IsNullOrEmpty(promptOverride)) return promptOverride;
            if (task != null) return task.title;
            return GameSettings.Current.defaultPrompt;
        }
    }

    public bool CanInteract
    {
        get
        {
            if (task == null) return false;
            if (TaskManager.Instance != null && TaskManager.Instance.IsCompleted(task)) return false;
            if (task.mustBePlaced && IsCarryingReward()) return false;
            return true;
        }
    }

    protected override MinigameSettings HoldSettings
    {
        get { return task.minigame; }
    }

    protected override bool CanStartHold()
    {
        return IsHoldTask() && CanInteract;
    }

    protected override bool CanBeginHold(bool justPressed)
    {
        if (CanAddReward()) return true;

        if (justPressed) GameEvents.RaiseMessage(GameSettings.Current.inventoryFullMessage);
        return false;
    }

    public void Interact()
    {
        if (!CanInteract || IsHoldTask()) return;

        if (!CanAddReward())
        {
            GameEvents.RaiseMessage(GameSettings.Current.inventoryFullMessage);
            return;
        }

        if (task.needsPower && !PowerSupply.HasPower)
        {
            GameEvents.RaiseMessage(GameSettings.Current.noPowerMessage);
            return;
        }

        if (MinigameManager.Instance == null)
        {
            Debug.LogWarning("Pickup: no minigame window in the scene. Add the MinigameWindow prefab to the scene.");
            return;
        }

        MinigameManager.Instance.Begin(task, OnMinigameFinished);
    }

    protected override void OnHoldComplete(float seconds)
    {
        if (task.mustBePlaced)
        {
            GiveReward();
            gameObject.SetActive(false);
            return;
        }

        if (TaskManager.Instance != null) TaskManager.Instance.RecordAttempt(task, true, seconds);
        OnMinigameFinished(true);
    }

    private bool IsHoldTask()
    {
        return task != null && task.minigame.type == MinigameType.HoldInWorld;
    }

    private bool IsCarryingReward()
    {
        return task.rewardItem != null && InventoryManager.Instance != null && InventoryManager.Instance.Contains(task.rewardItem);
    }

    private bool CanAddReward()
    {
        InventoryManager inventory = InventoryManager.Instance;
        return task.rewardItem == null || inventory == null || inventory.CanAdd(task.rewardItem);
    }

    private bool GiveReward()
    {
        InventoryManager inventory = InventoryManager.Instance;
        if (task.rewardItem == null || inventory == null || !inventory.TryAdd(task.rewardItem)) return false;

        GameEvents.RaiseMessage(GameSettings.Current.collectedMessage + task.rewardItem.displayName);
        return true;
    }

    private void OnMinigameFinished(bool success)
    {
        if (!success) return;

        GiveReward();

        if (hideWhenCompleted) gameObject.SetActive(false);
    }
}
