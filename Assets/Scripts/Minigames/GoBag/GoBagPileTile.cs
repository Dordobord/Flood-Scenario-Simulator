using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class GoBagPileTile : MonoBehaviour, IPointerDownHandler
{
    [Header("Parts")]
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI label;

    private GoBagItemView itemView;
    private GoBagMinigame owner;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private float emptyAlpha;
    private bool isAvailable = true;

    public GoBagItemView ItemView => itemView;

    public Vector2 Size => rectTransform.sizeDelta;

    public void Initialize(GoBagItemView view, GoBagMinigame minigame, GoBagSettings settings)
    {
        itemView = view;
        owner = minigame;
        emptyAlpha = settings.emptyTileAlpha;
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = (RectTransform)transform;

        GoBagItem item = view.Item;
        float inset = settings.cellGap * 0.5f;
        rectTransform.sizeDelta = new Vector2(item.BaseWidth * settings.cellSize - inset * 2f, item.BaseHeight * settings.cellSize - inset * 2f);

        if (icon != null && item.Icon != null) icon.sprite = item.Icon;

        if (label != null)
        {
            label.text = string.Format(settings.tileLabelFormat, item.Name, item.BaseWidth, item.BaseHeight);
            label.gameObject.SetActive(item.Icon == null);
        }

        gameObject.name = "Tile_" + item.Name;
        SetAvailable(true);
    }

    public void PlaceAt(Vector2 topLeft)
    {
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(0f, 1f);
        rectTransform.pivot = new Vector2(0f, 1f);
        rectTransform.anchoredPosition = topLeft;
    }

    public void SetAvailable(bool available)
    {
        isAvailable = available;
        canvasGroup.alpha = available ? 1f : emptyAlpha;
        canvasGroup.blocksRaycasts = available;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!isAvailable) return;

        owner.TryPickUpFromPile(this, eventData.position);
    }
}
