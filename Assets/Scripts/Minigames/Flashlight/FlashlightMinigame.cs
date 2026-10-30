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
    [SerializeField] private RectTransform usedPile;
    [SerializeField] private RectTransform dragLayer;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text countText;

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

    [Header("Drop Highlight")]
    [SerializeField] private Color highlightColor = new Color(0.247f, 0.682f, 0.353f);

    [Header("Text")]
    [SerializeField] private string statusFormat = "New batteries in: {0}/{1}";
    [SerializeField] private string countFormat = "{0} Left";
    [SerializeField] private string hintFormat = "Drag a battery. Press {0} to flip it while you hold it.";

    private readonly List<BatteryView> allBatteries = new List<BatteryView>();
    private readonly List<BatteryView> spares = new List<BatteryView>();
    private readonly List<BatteryView> tray = new List<BatteryView>();
    private readonly Dictionary<Outline, Color> outlineColors = new Dictionary<Outline, Color>();
    private readonly List<Vector3> pileSpots = new List<Vector3>();
    private readonly List<float> pileTilts = new List<float>();

    private bool isBuilt;
    private bool isOpen;
    private bool resetOnNextOpen;

    private BatteryView heldBattery;
    private BatterySlot originSlot;
    private List<BatteryView> originList;
    private int pickUpFrame = -1;
    private Outline highlighted;

    public override bool UsesConfirm
    {
        get { return true; }
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

        string hint = string.Format(hintFormat, flipKey);
        ShowInstructions(string.IsNullOrEmpty(settings.prompt) ? hint : settings.prompt + "\n" + hint);
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
        SetHighlight(GetDropOutline(mousePosition));

        if (Time.frameCount == pickUpFrame) return;

        if (!Mouse.current.leftButton.isPressed)
        {
            Drop(mousePosition);
        }
    }

    public void TryPickUp(BatteryView battery)
    {
        if (!isOpen || IsFinished || heldBattery != null || !MinigameManager.CursorFree) return;

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

    private void Drop(Vector2 mousePosition)
    {
        BatteryView battery = heldBattery;
        heldBattery = null;
        SetHighlight(null);

        BatterySlot slot = GetSlotAt(mousePosition);
        if (slot != null && slot.Battery == null)
        {
            PutInSlot(battery, slot);
            UpdateStatus();
            return;
        }

        if (battery.IsDead && IsOverTray(mousePosition))
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
        SetHighlight(null);
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

    public override void Confirm()
    {
        if (!isOpen || IsFinished || !isBuilt || heldBattery != null) return;

        int wrongSlots = 0;
        foreach (BatterySlot slot in slots)
        {
            if (!IsSlotCorrect(slot)) wrongSlots++;
        }

        Mistakes = wrongSlots;
        bool success = wrongSlots == 0;
        if (!success) resetOnNextOpen = true;
        if (success) AudioManager.PlaySfx(SoundLibrary.Current.battery);

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

    private bool IsOverTray(Vector2 screenPosition)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(usedTray, screenPosition, null);
    }

    private Outline GetDropOutline(Vector2 screenPosition)
    {
        BatterySlot slot = GetSlotAt(screenPosition);
        if (slot != null && slot.Battery == null) return slot.GetComponent<Outline>();

        if (heldBattery.IsDead && IsOverTray(screenPosition)) return usedTray.GetComponent<Outline>();

        return null;
    }

    private void SetHighlight(Outline outline)
    {
        if (outline == highlighted) return;

        if (highlighted != null) highlighted.effectColor = outlineColors[highlighted];

        highlighted = outline;
        if (highlighted == null) return;

        if (!outlineColors.ContainsKey(highlighted)) outlineColors[highlighted] = highlighted.effectColor;
        highlighted.effectColor = highlightColor;
    }

    private void PutInSlot(BatteryView battery, BatterySlot slot)
    {
        slot.Battery = battery;
        battery.PlaceAt(slot.Rect, Vector2.zero);
    }

    private void LayoutRow(List<BatteryView> row)
    {
        RectTransform area = row == tray ? usedPile : sparesArea;

        if (row == tray && pileSpots.Count > 0)
        {
            for (int i = 0; i < row.Count; i++)
            {
                int spot = i % pileSpots.Count;
                row[i].PlaceAt(area, Vector2.zero);
                row[i].transform.localPosition = pileSpots[spot];
                row[i].SetTilt(pileTilts[spot]);
            }
            return;
        }

        for (int i = 0; i < row.Count; i++)
        {
            float x = (i - (row.Count - 1) * 0.5f) * (row[i].Width + batterySpacing);
            row[i].PlaceAt(area, new Vector2(x, 0f));
        }
    }

    private void UpdateStatus()
    {
        if (slots == null) return;

        if (statusText != null)
        {
            int newInSlots = 0;
            foreach (BatterySlot slot in slots)
            {
                if (slot.Battery != null && !slot.Battery.IsDead) newInSlots++;
            }
            statusText.text = string.Format(statusFormat, newInSlots, slots.Length);
        }

        if (countText != null) countText.text = string.Format(countFormat, spares.Count);
    }

    private void Build()
    {
        if (dragLayer == null) dragLayer = GetCanvasLayer();

        if (slots == null || slots.Length == 0 || sparesArea == null || usedTray == null || usedPile == null
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

        SavePileSpots();
        ClearBlueprint(sparesArea);
        ClearBlueprint(usedPile);
        foreach (BatterySlot slot in slots) ClearBlueprint(slot.Rect);

        for (int i = 0; i < slots.Length; i++)
        {
            allBatteries.Add(CreateBattery(true, i));
            allBatteries.Add(CreateBattery(false, i));
        }

        isBuilt = true;
        ResetBatteries();
    }

    private void SavePileSpots()
    {
        foreach (Transform child in usedPile)
        {
            pileSpots.Add(child.localPosition);
            pileTilts.Add(child.localEulerAngles.z);
        }
    }

    private static void ClearBlueprint(RectTransform area)
    {
        LayoutGroup layoutGroup = area.GetComponent<LayoutGroup>();
        if (layoutGroup != null) layoutGroup.enabled = false;

        ClearChildren(area);
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
