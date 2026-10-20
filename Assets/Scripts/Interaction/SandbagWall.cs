using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SandbagSlot
{
    public GameObject placed;
    public GameObject ghost;
}

public class SandbagWall : MonoBehaviour, IInteractable
{
    [Header("Task")]
    [SerializeField] private TaskData task;

    [Header("Slots")]
    [SerializeField] private List<SandbagSlot> slots = new List<SandbagSlot>();

    private int placedCount;
    private float firstPlaceTime;

    public bool IsFull
    {
        get { return placedCount >= slots.Count; }
    }

    public string Prompt
    {
        get { return string.Format(GameSettings.Current.sandbagPlacePromptFormat, placedCount, slots.Count); }
    }

    public bool CanInteract
    {
        get { return IsCarrying(); }
    }

    private void Awake()
    {
        if (task == null || task.rewardItem == null)
        {
            Debug.LogWarning("SandbagWall: assign a Task that has a Reward Item.");
        }

        foreach (SandbagSlot slot in slots)
        {
            if (slot.placed != null)
            {
                if (slot.placed.GetComponent<Collider>() == null) slot.placed.AddComponent<BoxCollider>();
                slot.placed.SetActive(false);
            }
            if (slot.ghost != null) slot.ghost.SetActive(false);
        }
    }

    private void Update()
    {
        bool carrying = IsCarrying();

        for (int i = 0; i < slots.Count; i++)
        {
            GameObject ghost = slots[i].ghost;
            if (ghost == null) continue;

            bool showGhost = carrying && i == placedCount;
            if (ghost.activeSelf != showGhost) ghost.SetActive(showGhost);
        }
    }

    public void Interact()
    {
        if (IsCarrying()) PlaceSandbag();
    }

    private void PlaceSandbag()
    {
        if (InventoryManager.Instance != null) InventoryManager.Instance.Remove(task.rewardItem);
        if (placedCount == 0) firstPlaceTime = Time.time;

        SandbagSlot slot = slots[placedCount];
        if (slot.ghost != null) slot.ghost.SetActive(false);
        if (slot.placed != null) slot.placed.SetActive(true);
        placedCount++;

        GameSettings settings = GameSettings.Current;
        if (!IsFull)
        {
            GameEvents.RaiseMessage(string.Format(settings.sandbagPlacedMessageFormat, placedCount, slots.Count));
            return;
        }

        if (TaskManager.Instance != null) TaskManager.Instance.RecordAttempt(task, true, Time.time - firstPlaceTime);
    }

    private bool IsCarrying()
    {
        if (IsFull || task == null || task.rewardItem == null) return false;
        if (TaskManager.Instance != null && TaskManager.Instance.IsCompleted(task)) return false;
        return InventoryManager.Instance != null && InventoryManager.Instance.Contains(task.rewardItem);
    }
}
