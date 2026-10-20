using UnityEngine;

public class PlaceSpot : HoldInteractable, IInteractable
{
    [Header("Task")]
    [SerializeField] private TaskData task;

    [Header("Objects")]
    [SerializeField] private GameObject ghost;
    [SerializeField] private GameObject placedObject;
    [SerializeField] private Collider placeCollider;

    private bool placed;

    public string Prompt
    {
        get
        {
            GameSettings settings = GameSettings.Current;
            if (task == null || task.rewardItem == null)
            {
                return settings.defaultPrompt;
            }
            return string.Format(settings.placePromptFormat, task.rewardItem.displayName);
        }
    }

    public bool CanInteract
    {
        get { return IsCarrying(); }
    }

    protected override MinigameSettings HoldSettings
    {
        get { return task.minigame; }
    }

    private void Awake()
    {
        if (task == null || task.rewardItem == null)
        {
            Debug.LogWarning("PlaceSpot: assign a Task that has a Reward Item.");
        }

        if (placedObject != null) placedObject.SetActive(false);
        ShowGhost(false);
    }

    private void Update()
    {
        bool carrying = IsCarrying();
        if (ghost != null && ghost.activeSelf != carrying) ShowGhost(carrying);
    }

    public void Interact()
    {
    }

    protected override bool CanStartHold()
    {
        return IsCarrying();
    }

    protected override void OnHoldComplete(float seconds)
    {
        placed = true;

        if (InventoryManager.Instance != null) InventoryManager.Instance.Remove(task.rewardItem);
        ShowGhost(false);
        if (placedObject != null) placedObject.SetActive(true);

        if (TaskManager.Instance != null) TaskManager.Instance.RecordAttempt(task, true, seconds);
    }

    private bool IsCarrying()
    {
        if (placed || task == null || task.rewardItem == null) return false;
        if (TaskManager.Instance != null && TaskManager.Instance.IsCompleted(task)) return false;
        return InventoryManager.Instance != null && InventoryManager.Instance.Contains(task.rewardItem);
    }

    private void ShowGhost(bool show)
    {
        if (ghost != null) ghost.SetActive(show);
        if (placeCollider != null) placeCollider.enabled = show;
    }
}
