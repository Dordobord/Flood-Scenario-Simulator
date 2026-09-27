using System;
using System.Collections.Generic;
using UnityEngine;

namespace StormWaits
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [SerializeField, Min(1)] private int slotCount = 5;

        private List<ItemData> items = new List<ItemData>();

        public event Action Changed;

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

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
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
            GameEvents.RaiseItemAdded(item);
            return true;
        }

        public bool Has(ItemData item)
        {
            return items.Contains(item);
        }

        public bool Remove(ItemData item)
        {
            bool removed = items.Remove(item);
            if (removed && Changed != null) Changed();
            return removed;
        }

        public void Clear()
        {
            items.Clear();
            if (Changed != null) Changed();
        }
    }
}
