using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudUI : MonoBehaviour
{
    public static HudUI Instance { get; private set; }

    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text toastText;
    [SerializeField] private Transform hotbarParent;

    [Header("Hold Bar")]
    [SerializeField] private HoldProgressMinigame holdBar;

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

    public HoldProgressMinigame HoldBar
    {
        get { return holdBar; }
    }

    public HoldInteractable HoldBarUser { get; set; }

    private void Start()
    {
        if (holdBar != null) holdBar.End();
        if (toastText != null) toastText.text = "";

        FindSlots();

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Changed += RefreshHotbar;
            InventoryManager.Instance.SelectionChanged += RefreshHotbar;
        }
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.TaskCompleted += ShowTaskCompleted;
        }
        GameEvents.MessageRaised += ShowMessage;

        RefreshHotbar();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Changed -= RefreshHotbar;
            InventoryManager.Instance.SelectionChanged -= RefreshHotbar;
        }
        if (TaskManager.Instance != null)
        {
            TaskManager.Instance.TaskCompleted -= ShowTaskCompleted;
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

    private void ShowTaskCompleted(TaskData task)
    {
        ShowMessage(GameSettings.Current.taskCompleteMessage + task.title);
    }

    private void FindSlots()
    {
        slots.AddRange(hotbarParent.GetComponentsInChildren<InventorySlot>(true));

        for (int i = 0; i < slots.Count; i++)
        {
            slots[i].SetNumber(i + 1);
        }

        if (InventoryManager.Instance != null && InventoryManager.Instance.SlotCount != slots.Count)
        {
            Debug.LogWarning("HudUI: the Hotbar has " + slots.Count + " slots but the InventoryManager has " + InventoryManager.Instance.SlotCount + ". Make them match.");
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
            slots[i].SetSelected(inventory != null && i == inventory.SelectedIndex);
        }
    }
}
