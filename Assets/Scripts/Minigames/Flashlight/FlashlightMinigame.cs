using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FlashlightMinigame : MinigameBase
{
    [Header("References")]
    [SerializeField] private BatterySlot[] slots;
    [SerializeField] private RectTransform sparesArea;
    [SerializeField] private RectTransform usedTray;
    [SerializeField] private RectTransform dragLayer;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text hintText;

    [Header("Controls")]
    [SerializeField] private Key flipKey = Key.R;

    [Header("Batteries")]
    [SerializeField] private BatteryView newBatteryPrefab;
    [SerializeField] private BatteryView deadBatteryPrefab;
    [SerializeField] private float batterySpacing = 20f;
    [SerializeField, Range(0.1f, 1f)] private float heldAlpha = 0.8f;

    [Header("Starting directions")]
    [SerializeField] private bool randomizeDeadDirection = false;
    [SerializeField] private bool randomizeNewDirection = true;

    [Header("Text")]
    [SerializeField] private string statusFormat = "New batteries in: {0}/{1}";
    [SerializeField] private string hintFormat = "Click a battery to pick it up. {0} to flip it. Click a slot or the tray to put it down.";

    private readonly List<BatteryView> allBatteries = new List<BatteryView>();
    private readonly List<BatteryView> spares = new List<BatteryView>();
    private readonly List<BatteryView> tray = new List<BatteryView>();

    private bool isBuilt;
    private bool isOpen;
    private bool resetOnNextOpen;

    private BatteryView heldBattery;
    private BatterySlot originSlot;
    private List<BatteryView> originList;
    private int pickUpFrame = -1;
    private int lastPlaceClickFrame = -1;

    private void Awake()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }
        else
        {
            Debug.LogWarning("FlashlightMinigame: Confirm Button is not assigned in the Inspector.");
        }
    }

    protected override void Setup()
    {
        if (!isBuilt)
        {
            Build();
        }
        else if (resetOnNextOpen)
        {
            ResetBatteries();
        }
        resetOnNextOpen = false;

        if (hintText != null) hintText.text = string.Format(hintFormat, flipKey);
        UpdateStatus();

        isOpen = isBuilt;
    }

    public override void End()
    {
        CancelHeld();
        isOpen = false;
        base.End();
    }

    public override void Tick()
    {
        if (!isOpen) return;

        if (!MinigameManager.CursorFree)
        {
            CancelHeld();
            return;
        }

        if (heldBattery == null || Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (MinigameInput.KeyPressedThisFrame(flipKey))
        {
            heldBattery.Flip();
        }

        heldBattery.FollowPointer(mousePosition, dragLayer);

        if (Time.frameCount == pickUpFrame) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandlePlaceClick(mousePosition);
        }
    }

    public void TryPickUp(BatteryView battery)
    {
        if (!isOpen || IsFinished || heldBattery != null || !MinigameManager.CursorFree) return;
        if (Time.frameCount == lastPlaceClickFrame) return;

        originSlot = null;
        originList = null;

        foreach (BatterySlot slot in slots)
        {
            if (slot.Battery == battery)
            {
                slot.Battery = null;
                originSlot = slot;
            }
        }

        if (originSlot == null)
        {
            if (spares.Remove(battery)) originList = spares;
            else if (tray.Remove(battery)) originList = tray;

            if (originList != null) LayoutRow(originList);
        }

        heldBattery = battery;
        pickUpFrame = Time.frameCount;
        battery.BeginHold(dragLayer, heldAlpha);
        UpdateStatus();
    }

    private void HandlePlaceClick(Vector2 mousePosition)
    {
        lastPlaceClickFrame = Time.frameCount;

        BatteryView battery = heldBattery;
        heldBattery = null;

        BatterySlot slot = GetSlotAt(mousePosition);
        if (slot != null && slot.Battery == null)
        {
            PutInSlot(battery, slot);
            UpdateStatus();
            return;
        }

        if (battery.IsDead && RectTransformUtility.RectangleContainsScreenPoint(usedTray, mousePosition, null))
        {
            tray.Add(battery);
            LayoutRow(tray);
            UpdateStatus();
            return;
        }

        ReturnToOrigin(battery);
    }

    private void CancelHeld()
    {
        if (heldBattery == null) return;

        ReturnToOrigin(heldBattery);
        heldBattery = null;
    }

    private void ReturnToOrigin(BatteryView battery)
    {
        if (originSlot != null)
        {
            PutInSlot(battery, originSlot);
        }
        else if (originList != null)
        {
            originList.Add(battery);
            LayoutRow(originList);
        }
        else
        {
            spares.Add(battery);
            LayoutRow(spares);
        }
        UpdateStatus();
    }

    private void OnConfirmClicked()
    {
        if (!isOpen || IsFinished || !isBuilt) return;

        if (heldBattery != null || Time.frameCount == lastPlaceClickFrame) return;

        int wrongSlots = 0;
        foreach (BatterySlot slot in slots)
        {
            if (!IsSlotCorrect(slot)) wrongSlots++;
        }

        Mistakes = wrongSlots;
        bool success = wrongSlots == 0;
        if (!success) resetOnNextOpen = true;

        Finish(success);
    }

    private bool IsSlotCorrect(BatterySlot slot)
    {
        BatteryView battery = slot.Battery;
        return battery != null && !battery.IsDead && battery.PlusUp == slot.PlusFacesUp;
    }

    private BatterySlot GetSlotAt(Vector2 screenPosition)
    {
        foreach (BatterySlot slot in slots)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(slot.Rect, screenPosition, null)) return slot;
        }
        return null;
    }

    private void PutInSlot(BatteryView battery, BatterySlot slot)
    {
        slot.Battery = battery;
        battery.PlaceAt(slot.Rect, Vector2.zero);
    }

    private void LayoutRow(List<BatteryView> row)
    {
        RectTransform area = row == tray ? usedTray : sparesArea;

        for (int i = 0; i < row.Count; i++)
        {
            float x = (i - (row.Count - 1) * 0.5f) * (row[i].Width + batterySpacing);
            row[i].PlaceAt(area, new Vector2(x, 0f));
        }
    }

    private void UpdateStatus()
    {
        if (statusText == null || slots == null) return;

        int newInSlots = 0;
        foreach (BatterySlot slot in slots)
        {
            if (slot.Battery != null && !slot.Battery.IsDead) newInSlots++;
        }
        statusText.text = string.Format(statusFormat, newInSlots, slots.Length);
    }

    private void Build()
    {
        if (slots == null || slots.Length == 0 || sparesArea == null || usedTray == null || dragLayer == null
            || newBatteryPrefab == null || deadBatteryPrefab == null)
        {
            Debug.LogError("FlashlightMinigame: a field is empty in the Inspector.");
            return;
        }

        foreach (BatterySlot slot in slots)
        {
            if (slot == null)
            {
                Debug.LogError("FlashlightMinigame: one of the Slots is empty in the Inspector.");
                return;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            allBatteries.Add(CreateBattery(true, i));
            allBatteries.Add(CreateBattery(false, i));
        }

        isBuilt = true;
        ResetBatteries();
    }

    private BatteryView CreateBattery(bool isDead, int index)
    {
        BatteryView prefab = isDead ? deadBatteryPrefab : newBatteryPrefab;
        BatteryView battery = Instantiate(prefab, sparesArea);
        battery.name = (isDead ? "DeadBattery_" : "NewBattery_") + index;
        battery.Initialize(this, isDead);
        return battery;
    }

    private void ResetBatteries()
    {
        heldBattery = null;
        spares.Clear();
        tray.Clear();
        foreach (BatterySlot slot in slots) slot.Battery = null;

        int slotIndex = 0;
        foreach (BatteryView battery in allBatteries)
        {
            if (battery.IsDead)
            {
                BatterySlot slot = slots[slotIndex];
                battery.SetPlusUp(randomizeDeadDirection ? Random.value > 0.5f : slot.PlusFacesUp);
                PutInSlot(battery, slot);
                slotIndex++;
            }
            else
            {
                battery.SetPlusUp(randomizeNewDirection ? Random.value > 0.5f : true);
                spares.Add(battery);
            }
        }

        LayoutRow(spares);
        LayoutRow(tray);
    }
}
