using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HudUI : MonoBehaviour
{
    public static HudUI Instance { get; private set; }

    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text toastText;
    [SerializeField] private TMP_Text hazardText;
    [SerializeField] private Transform hotbarParent;

    [Header("Hold Bar")]
    [SerializeField] private HoldProgressMinigame holdBar;

    [Header("Phone Alert")]
    [SerializeField] private Color blinkColorA = Color.white;
    [SerializeField] private Color blinkColorB = new Color(0.89f, 0.29f, 0.29f);

    private List<InventorySlot> slots = new List<InventorySlot>();
    private float toastHideTime;
    private ItemData blinkItem;
    private Queue<string> waitingMessages = new Queue<string>();

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
        if (hazardText != null) hazardText.text = "";

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
        GameEvents.EvacuationCompleted += ClearHazardText;

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
        GameEvents.EvacuationCompleted -= ClearHazardText;

        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        bool expired = toastHideTime > 0f && Time.time >= toastHideTime;
        if (expired || PhaseManager.IsTransitionActive) HideMessage();

        ShowWaitingMessage();
        UpdateBlink();
    }

    public void BlinkItem(ItemData item)
    {
        blinkItem = item;
    }

    private void UpdateBlink()
    {
        if (blinkItem == null || InventoryManager.Instance == null) return;

        InventoryManager inventory = InventoryManager.Instance;
        bool isPreparing = PhaseManager.Instance != null && PhaseManager.Instance.CurrentPhase == GamePhase.Preparation;

        if (!isPreparing || MinigameManager.IsPhoneOpen)
        {
            blinkItem = null;
            RefreshHotbar();
            return;
        }

        GameSettings settings = GameSettings.Current;
        bool showFirst = Mathf.Repeat(Time.time, settings.phoneAlertBlinkSeconds * 2f) < settings.phoneAlertBlinkSeconds;
        int index = inventory.Items.IndexOf(blinkItem);

        if (index >= 0 && index < slots.Count) slots[index].SetBlinkColor(showFirst ? blinkColorA : blinkColorB);
    }

    private void HideMessage()
    {
        if (toastText.text == "") return;

        toastText.text = "";
        toastHideTime = 0f;
    }

    public void SetPrompt(string text)
    {
        if (promptText.text != text) promptText.text = text;
    }

    public void SetHazardText(string text)
    {
        if (hazardText != null) hazardText.text = text;
    }

    private void ClearHazardText()
    {
        SetHazardText("");
    }

    public void ShowMessage(string message)
    {
        if (MinigameManager.IsActive)
        {
            waitingMessages.Enqueue(message);
            return;
        }

        DisplayMessage(message);
    }

    private void DisplayMessage(string message)
    {
        toastText.text = message;
        toastHideTime = Time.time + GameSettings.Current.toastSeconds;
    }

    private void ShowWaitingMessage()
    {
        if (waitingMessages.Count == 0 || MinigameManager.IsActive || toastHideTime > 0f) return;

        DisplayMessage(waitingMessages.Dequeue());
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
