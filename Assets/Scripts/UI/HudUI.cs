using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StormWaits
{
    
    
    
    public class HudUI : MonoBehaviour
    {
        public static HudUI Instance { get; private set; }

        [SerializeField] private TMP_Text promptText;
        [SerializeField] private TMP_Text toastText;
        [SerializeField] private TMP_Text taskListText;
        [SerializeField] private Transform hotbarParent;
        [SerializeField] private InventorySlot slotPrefab;

        private List<InventorySlot> slots = new List<InventorySlot>();
        private float toastHideTime;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            BuildHotbar();

            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.Changed += RefreshHotbar;
            }
            if (TaskManager.Instance != null)
            {
                TaskManager.Instance.Changed += RefreshTasks;
            }
            GameEvents.MessageRaised += ShowMessage;

            RefreshHotbar();
            RefreshTasks();
        }

        private void OnDestroy()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.Changed -= RefreshHotbar;
            }
            if (TaskManager.Instance != null)
            {
                TaskManager.Instance.Changed -= RefreshTasks;
            }
            GameEvents.MessageRaised -= ShowMessage;

            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            
            if (toastHideTime > 0f && Time.time >= toastHideTime)
            {
                toastText.text = "";
                toastHideTime = 0f;
            }
        }

        public void SetPrompt(string text)
        {
            if (promptText.text != text) promptText.text = text;
        }

        public void ShowMessage(string message)
        {
            toastText.text = message;
            toastHideTime = Time.time + GameSettings.Current.toastSeconds;
        }

        
        private void BuildHotbar()
        {
            int slotCount = 5;
            if (InventoryManager.Instance != null)
            {
                slotCount = InventoryManager.Instance.SlotCount;
            }

            for (int i = 0; i < slotCount; i++)
            {
                InventorySlot slot = Instantiate(slotPrefab, hotbarParent);
                slots.Add(slot);
            }
        }

        private void RefreshHotbar()
        {
            InventoryManager inventory = InventoryManager.Instance;

            for (int i = 0; i < slots.Count; i++)
            {
                ItemData item = null;
                if (inventory != null && i < inventory.Items.Count)
                {
                    item = inventory.Items[i];
                }
                slots[i].SetItem(item);
            }
        }

        private void RefreshTasks()
        {
            TaskManager tasks = TaskManager.Instance;
            if (tasks == null)
            {
                taskListText.text = "";
                return;
            }

            GameSettings settings = GameSettings.Current;
            string doneColor = ColorUtility.ToHtmlStringRGB(settings.taskDoneColor);

            string text = "<b>" + string.Format(settings.taskHeaderFormat, tasks.CompletedCount, tasks.TotalCount) + "</b>\n";

            foreach (TaskData task in tasks.ActiveTasks)
            {
                if (tasks.IsCompleted(task))
                {
                    text += "<color=#" + doneColor + ">" + settings.doneMark + task.title + "</color>\n";
                }
                else
                {
                    text += settings.todoMark + task.title + "\n";
                }
            }

            taskListText.text = text;
        }
    }
}
