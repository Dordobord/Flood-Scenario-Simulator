using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class GoBagItemView : MonoBehaviour, IPointerDownHandler
{
    [Header("Parts")]
    [SerializeField] private Image iconImage;

    private GoBagItem item;
    private GoBagMinigame owner;
    private GoBagPileTile tile;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private float cellSize;
    private float inset;
    private float heldAlpha;
    private RectTransform iconRect;
    private float iconPadding;

    private bool isHeld;
    private bool pickedFromPile;
    private Transform originParent;
    private Vector2 originPosition;
    private bool wasRotatedAtPickUp;
    private Vector3 grabOffset;

    public GoBagItem Item => item;

    public void Initialize(GoBagItem goBagItem, GoBagMinigame minigame, GoBagSettings settings)
    {
        item = goBagItem;
        owner = minigame;
        cellSize = settings.cellSize;
        inset = settings.cellGap * 0.5f;
        heldAlpha = settings.heldItemAlpha;

        rectTransform = (RectTransform)transform;
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        SetupIcon(settings);
        UpdateSize();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        gameObject.SetActive(false);
    }

    public void SetTile(GoBagPileTile pileTile)
    {
        tile = pileTile;
    }

    public void UpdateSize()
    {
        rectTransform.sizeDelta = new Vector2(item.Width * cellSize - inset * 2f, item.Height * cellSize - inset * 2f);

        if (iconRect == null) return;

        iconRect.sizeDelta = new Vector2(item.BaseWidth * cellSize - inset * 2f - iconPadding * 2f, item.BaseHeight * cellSize - inset * 2f - iconPadding * 2f);
        iconRect.localEulerAngles = new Vector3(0f, 0f, item.IsRotated ? 90f : 0f);
    }

    private void SetupIcon(GoBagSettings settings)
    {
        if (iconImage == null || item.Icon == null)
        {
            if (iconImage != null) iconImage.gameObject.SetActive(false);

            Image background = GetComponent<Image>();
            background.enabled = true;
            background.color = settings.itemColor;
            CreateLabel(settings);
            return;
        }

        iconImage.sprite = item.Icon;
        iconRect = iconImage.rectTransform;
        iconPadding = iconRect.anchorMin != iconRect.anchorMax ? -iconRect.sizeDelta.x * 0.5f : 0f;
        iconRect.anchorMin = new Vector2(0.5f, 0.5f);
        iconRect.anchorMax = new Vector2(0.5f, 0.5f);
        iconRect.anchoredPosition = Vector2.zero;
    }

    private void CreateLabel(GoBagSettings settings)
    {
        GameObject labelObject = new GameObject("Label", typeof(RectTransform), typeof(TextMeshProUGUI));
        labelObject.layer = gameObject.layer;

        RectTransform labelRect = (RectTransform)labelObject.transform;
        labelRect.SetParent(transform, false);
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = new Vector2(settings.labelPadding, settings.labelPadding);
        labelRect.offsetMax = new Vector2(-settings.labelPadding, -settings.labelPadding);

        TextMeshProUGUI label = labelObject.GetComponent<TextMeshProUGUI>();
        label.text = item.Name;
        label.alignment = TextAlignmentOptions.Center;
        label.enableAutoSizing = true;
        label.fontSizeMin = settings.labelMinFontSize;
        label.fontSizeMax = settings.labelMaxFontSize;
        label.color = settings.labelColor;
        label.raycastTarget = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        owner.TryPickUpFromBag(this, eventData.position);
    }

    public void PickUpFromPile(Vector2 screenPosition)
    {
        gameObject.SetActive(true);
        transform.SetParent(owner.DragLayer, false);
        StartHolding(true);

        grabOffset = rectTransform.TransformVector(
            new Vector3(-rectTransform.sizeDelta.x * 0.5f, rectTransform.sizeDelta.y * 0.5f, 0f));
        FollowPointer(screenPosition);
    }

    public void PickUpFromBag(Vector2 screenPosition)
    {
        transform.SetParent(owner.DragLayer, true);
        StartHolding(false);

        if (GetPointerWorldPosition(screenPosition, out Vector3 pointer))
        {
            grabOffset = rectTransform.position - pointer;
        }
        else
        {
            grabOffset = Vector3.zero;
        }
    }

    private void StartHolding(bool fromPile)
    {
        isHeld = true;
        pickedFromPile = fromPile;
        originParent = transform.parent;
        originPosition = rectTransform.anchoredPosition;
        wasRotatedAtPickUp = item.IsRotated;

        transform.SetAsLastSibling();
        canvasGroup.alpha = heldAlpha;
        canvasGroup.blocksRaycasts = false;
    }

    public void FollowPointer(Vector2 screenPosition)
    {
        if (!isHeld) return;

        if (GetPointerWorldPosition(screenPosition, out Vector3 pointer))
        {
            rectTransform.position = pointer + grabOffset;
        }
    }

    public void RotateHeld()
    {
        if (!isHeld) return;
        if (!item.CanRotate) return;

        item.Rotate();
        UpdateSize();
        grabOffset = new Vector3(-grabOffset.y, -grabOffset.x, 0f);
    }

    public void MoveTo(Transform parent, Vector2 cellTopLeft)
    {
        Release();
        transform.SetParent(parent, false);
        rectTransform.anchoredPosition = cellTopLeft + new Vector2(inset, -inset);
    }

    public void ReturnHome()
    {
        Release();
        if (item.IsRotated)
        {
            item.Rotate();
            UpdateSize();
        }
        transform.SetParent(owner.DragLayer, false);
        gameObject.SetActive(false);

        if (tile != null) tile.SetAvailable(true);
    }

    public void CancelHold()
    {
        if (!isHeld) return;

        if (pickedFromPile)
        {
            ReturnHome();
            return;
        }

        Release();
        if (item.IsRotated != wasRotatedAtPickUp)
        {
            item.Rotate();
            UpdateSize();
        }
        transform.SetParent(originParent, false);
        rectTransform.anchoredPosition = originPosition;
    }

    private void Release()
    {
        isHeld = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public Vector2 GetTopLeftCellScreenPoint()
    {
        float half = cellSize * 0.5f - inset;
        Vector3 worldPoint = rectTransform.TransformPoint(new Vector3(half, -half, 0f));
        return RectTransformUtility.WorldToScreenPoint(null, worldPoint);
    }

    private bool GetPointerWorldPosition(Vector2 screenPosition, out Vector3 worldPoint)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(owner.DragLayer, screenPosition, null, out worldPoint);
    }
}
