using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class GoBagPileTile : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private TextMeshProUGUI label;

    private GoBagItemView itemView;
    private GoBagMinigame owner;
    private CanvasGroup canvasGroup;
    private float emptyAlpha;
    private bool isAvailable = true;

    public GoBagItemView ItemView => itemView;

    public void Initialize(GoBagItemView view, GoBagMinigame minigame, GoBagSettings settings)
    {
        itemView = view;
        owner = minigame;
        emptyAlpha = settings.emptyTileAlpha;
        canvasGroup = GetComponent<CanvasGroup>();

        GoBagItem item = view.Item;
        if (label != null)
        {
            label.text = string.Format(settings.tileLabelFormat, item.Name, item.BaseWidth, item.BaseHeight);
        }
        else
        {
            Debug.LogWarning("GoBagPileTile: Label is not assigned on the tile prefab.");
        }

        gameObject.name = "Tile_" + item.Name;
        SetAvailable(true);
    }

    public void SetAvailable(bool available)
    {
        isAvailable = available;
        canvasGroup.alpha = available ? 1f : emptyAlpha;
        canvasGroup.blocksRaycasts = available;
        if (label != null) label.enabled = available;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!isAvailable) return;

        owner.TryPickUpFromPile(this, eventData.position);
    }
}
