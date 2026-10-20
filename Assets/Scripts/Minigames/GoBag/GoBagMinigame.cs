using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GoBagMinigame : MinigameBase
{
    [SerializeField] private GoBagSettings goBagSettings;
    [SerializeField] private GoBagPileTile tilePrefab;

    [Header("References")]
    [SerializeField] private RectTransform bagArea;
    [SerializeField] private GoBagGridView gridView;
    [SerializeField] private RectTransform itemLayer;
    [SerializeField] private RectTransform pileArea;
    [SerializeField] private RectTransform dragLayer;
    [SerializeField] private Button confirmButton;

    private GoBagGrid bagGrid;
    private readonly List<GoBagItem> items = new List<GoBagItem>();

    private bool isOpen;

    private GoBagItemView heldItem;
    private int pickUpFrame = -1;
    private int lastPlaceClickFrame = -1;

    public RectTransform DragLayer => dragLayer;

    private void Awake()
    {
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmClicked);
        }
        else
        {
            Debug.LogWarning("GoBagMinigame: Confirm Button is not assigned in the Inspector.");
        }
    }

    protected override void Setup()
    {
        Build();
        isOpen = true;
    }

    private void OnConfirmClicked()
    {
        if (!isOpen || IsFinished || bagGrid == null) return;

        if (heldItem != null || Time.frameCount == lastPlaceClickFrame) return;

        GoBagResult result = GetResult();
        if (GameSettings.Current.logInput) Debug.Log("Go bag confirm: " + result.Describe());

        Finish(result.IsComplete);
    }

    public override void End()
    {
        if (isOpen)
        {
            CancelHeldItem();
            isOpen = false;
        }
        base.End();
    }

    private GoBagResult GetResult()
    {
        List<SelectChoice> missingItems = new List<SelectChoice>();

        foreach (GoBagItem item in items)
        {
            if (!bagGrid.Contains(item)) missingItems.Add(item.Choice);
        }

        return new GoBagResult(missingItems);
    }

    public override void Tick()
    {
        if (!isOpen || bagGrid == null) return;

        if (!MinigameManager.CursorFree)
        {
            CancelHeldItem();
            return;
        }

        if (heldItem == null || Mouse.current == null) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        if (MinigameInput.KeyPressedThisFrame(goBagSettings.rotateKey))
        {
            heldItem.RotateHeld();
        }

        heldItem.FollowPointer(mousePosition);
        UpdatePreview(mousePosition);

        if (Time.frameCount == pickUpFrame) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandlePlaceClick(mousePosition);
        }
    }

    public void TryPickUpFromPile(GoBagPileTile tile, Vector2 screenPosition)
    {
        if (!CanPickUpNow()) return;

        heldItem = tile.ItemView;
        pickUpFrame = Time.frameCount;
        tile.SetAvailable(false);
        heldItem.PickUpFromPile(screenPosition);
    }

    public void TryPickUpFromBag(GoBagItemView view, Vector2 screenPosition)
    {
        if (!CanPickUpNow()) return;

        heldItem = view;
        pickUpFrame = Time.frameCount;
        view.PickUpFromBag(screenPosition);
    }

    private bool CanPickUpNow()
    {
        if (!isOpen || IsFinished || heldItem != null || !MinigameManager.CursorFree) return false;
        return Time.frameCount != lastPlaceClickFrame;
    }

    private void CancelHeldItem()
    {
        if (heldItem != null)
        {
            heldItem.CancelHold();
            heldItem = null;
        }
        if (gridView != null) gridView.HidePreview();
    }

    private void HandlePlaceClick(Vector2 mousePosition)
    {
        lastPlaceClickFrame = Time.frameCount;

        if (gridView.TryGetCellFromScreen(mousePosition, out _, out _))
        {
            if (TryGetTargetCell(out int x, out int y) && bagGrid.Place(heldItem.Item, x, y))
            {
                heldItem.MoveTo(itemLayer, gridView.GetCellPosition(x, y));
                heldItem = null;
                gridView.HidePreview();
            }
            return;
        }

        bagGrid.Remove(heldItem.Item);
        heldItem.ReturnHome();
        heldItem = null;
        gridView.HidePreview();
    }

    private void UpdatePreview(Vector2 mousePosition)
    {
        if (!gridView.TryGetCellFromScreen(mousePosition, out _, out _) || !TryGetTargetCell(out int x, out int y))
        {
            gridView.HidePreview();
            return;
        }

        GoBagItem item = heldItem.Item;
        gridView.ShowPreview(x, y, item.Width, item.Height, bagGrid.CanPlace(item, x, y));
    }

    private bool TryGetTargetCell(out int x, out int y)
    {
        return gridView.TryGetCellFromScreenUnclamped(heldItem.GetTopLeftCellScreenPoint(), out x, out y);
    }

    private void Build()
    {
        if (bagGrid != null) return;

        if (settings == null || goBagSettings == null || tilePrefab == null || bagArea == null
            || gridView == null || itemLayer == null || pileArea == null || dragLayer == null)
        {
            Debug.LogError("GoBagMinigame: a field is empty in the Inspector.");
            return;
        }

        bagGrid = new GoBagGrid(goBagSettings.gridWidth, goBagSettings.gridHeight);

        foreach (SelectChoice choice in settings.choices)
        {
            if (choice != null) items.Add(new GoBagItem(choice));
        }

        Vector2 bagSize = new Vector2(goBagSettings.gridWidth * goBagSettings.cellSize, goBagSettings.gridHeight * goBagSettings.cellSize);
        CheckDesign(bagSize);

        PinTopLeft((RectTransform)gridView.transform, bagSize);
        PinTopLeft(itemLayer, bagSize);

        gridView.Build(goBagSettings);
        CreateItems();
    }

    private void CheckDesign(Vector2 expected)
    {
        int bagCells = goBagSettings.gridWidth * goBagSettings.gridHeight;
        int itemCells = 0;
        foreach (GoBagItem item in items)
        {
            itemCells += item.Width * item.Height;
        }

        if (items.Count == 0)
        {
            Debug.LogWarning("GoBagMinigame: the task has no choices, so an empty bag counts as complete.");
        }
        else if (itemCells > bagCells)
        {
            Debug.LogWarning("GoBagMinigame: the items take " + itemCells + " cells but the bag only has " + bagCells + ". Make items smaller or the bag bigger.");
        }
        else if (!GoBagGrid.CanFitAll(goBagSettings.gridWidth, goBagSettings.gridHeight, items, out GoBagItem misfit))
        {
            Debug.LogWarning("GoBagMinigame: the items may not fit in the bag (could not fit '" + misfit.Name + "'). Pack them by hand to check.");
        }

        Vector2 actual = bagArea.rect.size;
        if (Mathf.Abs(actual.x - expected.x) > 0.5f || Mathf.Abs(actual.y - expected.y) > 0.5f)
        {
            Debug.LogWarning("GoBagMinigame: BagArea is " + actual.x + "x" + actual.y + " but should be " + expected.x + "x" + expected.y + ".");
        }
    }

    private void CreateItems()
    {
        foreach (GoBagItem item in items)
        {
            GameObject viewObject = new GameObject("ItemView_" + item.Name, typeof(RectTransform), typeof(Image), typeof(GoBagItemView));
            viewObject.layer = gameObject.layer;
            viewObject.transform.SetParent(dragLayer, false);

            GoBagItemView view = viewObject.GetComponent<GoBagItemView>();
            view.Initialize(item, this, goBagSettings);

            GoBagPileTile tile = Instantiate(tilePrefab, pileArea);
            tile.Initialize(view, this, goBagSettings);
            view.SetTile(tile);
        }
    }

    private static void PinTopLeft(RectTransform rect, Vector2 size)
    {
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = size;
    }
}
