using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class FirstAidItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item")]
    [SerializeField] private string itemName = "New Item";
    [SerializeField] private Sprite sprite;
    [SerializeField] private bool isNeeded = true;

    [Header("Parts")]
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text nameText;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Image background;
    private Outline outline;
    private RectTransform dragLayer;
    private Transform startParent;
    private Vector2 startPosition;
    private int startSiblingIndex;
    private Vector3 grabOffset;
    private bool isDragging;

    public string ItemName
    {
        get { return itemName; }
    }

    public bool IsNeeded
    {
        get { return isNeeded; }
    }

    public Image Image
    {
        get { return image; }
    }

    public bool IsPlaced { get; private set; }

    private void Awake()
    {
        rectTransform = (RectTransform)transform;
        canvasGroup = GetComponent<CanvasGroup>();
        background = GetComponent<Image>();
        outline = GetComponent<Outline>();
        startParent = transform.parent;
        startPosition = rectTransform.anchoredPosition;
        startSiblingIndex = transform.GetSiblingIndex();
    }

    private void OnValidate()
    {
        ShowItem();

        FirstAidSlot[] slots = transform.root.GetComponentsInChildren<FirstAidSlot>(true);
        foreach (FirstAidSlot slot in slots)
        {
            if (slot.Item == this) slot.RefreshLater();
        }
    }

    public void ShowItem()
    {
        if (nameText != null) nameText.text = itemName;
        if (image != null && sprite != null) image.sprite = sprite;
    }

    public void SetDragLayer(RectTransform layer)
    {
        dragLayer = layer;
    }

    public void ResetItem()
    {
        IsPlaced = false;
        isDragging = false;
        canvasGroup.blocksRaycasts = true;
        ReturnToStart();
        SetCardVisible(true);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsPlaced || dragLayer == null) return;

        isDragging = true;
        transform.SetParent(dragLayer, true);
        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;
        SetCardVisible(false);

        Vector3 pointer;
        grabOffset = GetPointer(eventData, out pointer) ? rectTransform.position - pointer : Vector3.zero;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Vector3 pointer;
        if (GetPointer(eventData, out pointer)) rectTransform.position = pointer + grabOffset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        canvasGroup.blocksRaycasts = true;
        if (IsPlaced) return;

        ReturnToStart();
        SetCardVisible(true);
    }

    private void SetCardVisible(bool visible)
    {
        if (background != null) background.enabled = visible;
        if (outline != null) outline.enabled = visible;
        if (nameText != null) nameText.enabled = visible;
    }

    public void PlaceIn(RectTransform slot)
    {
        IsPlaced = true;
        transform.SetParent(slot, true);

        Vector3 centerOffset = rectTransform.position - rectTransform.TransformPoint(rectTransform.rect.center);
        rectTransform.position = slot.TransformPoint(slot.rect.center) + centerOffset;
    }

    private void ReturnToStart()
    {
        transform.SetParent(startParent, false);
        transform.SetSiblingIndex(startSiblingIndex);
        rectTransform.anchoredPosition = startPosition;
    }

    private bool GetPointer(PointerEventData eventData, out Vector3 pointer)
    {
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(dragLayer, eventData.position, eventData.pressEventCamera, out pointer);
    }
}
