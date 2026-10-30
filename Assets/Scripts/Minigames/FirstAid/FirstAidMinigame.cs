using TMPro;
using UnityEngine;

public class FirstAidMinigame : MinigameBase
{
    [Header("First Aid")]
    [SerializeField] private RectTransform dragLayer;
    [SerializeField] private TMP_Text countText;

    [Header("Text")]
    [SerializeField] private string countFormat = "{0} / {1}";

    private FirstAidItem[] items;
    private FirstAidSlot[] slots;

    private void Awake()
    {
        if (dragLayer == null) dragLayer = GetCanvasLayer();

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

        UpdateCount();
    }

    public override void Tick()
    {
    }

    private void CheckKit()
    {
        AudioManager.PlaySfx(SoundLibrary.Current.itemDrop);
        UpdateCount();

        foreach (FirstAidItem item in items)
        {
            if (item.IsNeeded && !item.IsPlaced) return;
        }

        Finish(true);
    }

    private void UpdateCount()
    {
        if (countText == null) return;

        int filled = 0;
        foreach (FirstAidSlot slot in slots)
        {
            if (slot.IsFilled) filled++;
        }
        countText.text = string.Format(countFormat, filled, slots.Length);
    }
}
