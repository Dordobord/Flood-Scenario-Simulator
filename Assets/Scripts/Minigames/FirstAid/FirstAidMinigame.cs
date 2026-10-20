using UnityEngine;

public class FirstAidMinigame : MinigameBase
{
    [Header("First Aid")]
    [SerializeField] private RectTransform dragLayer;

    private FirstAidItem[] items;
    private FirstAidSlot[] slots;

    private void Awake()
    {
        if (dragLayer == null) dragLayer = (RectTransform)transform;

        items = GetComponentsInChildren<FirstAidItem>(true);
        foreach (FirstAidItem item in items)
        {
            item.SetDragLayer(dragLayer);
        }

        slots = GetComponentsInChildren<FirstAidSlot>(true);
        foreach (FirstAidSlot slot in slots)
        {
            slot.Filled += CheckKit;
        }
    }

    protected override void Setup()
    {
        foreach (FirstAidItem item in items)
        {
            item.ResetItem();
        }

        foreach (FirstAidSlot slot in slots)
        {
            slot.ResetSlot();
        }
    }

    public override void Tick()
    {
    }

    private void CheckKit()
    {
        foreach (FirstAidItem item in items)
        {
            if (item.IsNeeded && !item.IsPlaced) return;
        }

        Finish(true);
    }
}
