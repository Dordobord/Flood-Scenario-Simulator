using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [SerializeField, Min(1)] private int slotCount = 5;

    [Header("Start")]
    [SerializeField] private List<ItemData> startingItems = new List<ItemData>();

    private List<ItemData> items = new List<ItemData>();
    private int selectedIndex;

    public event Action Changed;
    public event Action SelectionChanged;

    public int SlotCount
    {
        get { return slotCount; }
    }

    public List<ItemData> Items
    {
        get { return items; }
    }

    public bool IsFull
    {
        get { return items.Count >= slotCount; }
    }

    public int SelectedIndex
    {
        get { return selectedIndex; }
    }

    public ItemData SelectedItem
    {
        get { return selectedIndex < items.Count ? items[selectedIndex] : null; }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        foreach (ItemData item in startingItems)
        {
            if (item != null && !IsFull) items.Add(item);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Select(int index)
    {
        selectedIndex = (index % slotCount + slotCount) % slotCount;
        if (SelectionChanged != null) SelectionChanged();
    }

    public bool CanAdd(ItemData item)
    {
        return item != null && !IsFull;
    }

    public bool TryAdd(ItemData item)
    {
        if (!CanAdd(item)) return false;

        items.Add(item);
        if (Changed != null) Changed();
        return true;
    }

    public bool Contains(ItemData item)
    {
        return item != null && items.Contains(item);
    }

    public bool Remove(ItemData item)
    {
        if (item == null || !items.Remove(item)) return false;

        if (Changed != null) Changed();
        return true;
    }
}
