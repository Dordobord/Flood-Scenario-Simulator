using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class BatteryView : MonoBehaviour, IPointerDownHandler
{
    private FlashlightMinigame owner;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;

    public bool IsDead { get; private set; }
    public bool PlusUp { get; private set; }

    public float Width => rectTransform.sizeDelta.x;

    public void Initialize(FlashlightMinigame minigame, bool isDead)
    {
        owner = minigame;
        IsDead = isDead;

        rectTransform = (RectTransform)transform;

        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        SetPlusUp(true);
    }

    public void SetPlusUp(bool up)
    {
        PlusUp = up;
        SetTilt(0f);
    }

    public void SetTilt(float degrees)
    {
        rectTransform.localEulerAngles = new Vector3(0f, 0f, (PlusUp ? 0f : 180f) + degrees);
    }

    public void Flip()
    {
        SetPlusUp(!PlusUp);
    }

    public void BeginHold(RectTransform dragLayer, float alpha)
    {
        transform.SetParent(dragLayer, true);
        transform.SetAsLastSibling();
        SetTilt(0f);
        canvasGroup.alpha = alpha;
        canvasGroup.blocksRaycasts = false;
    }

    public void FollowPointer(Vector2 screenPosition, RectTransform dragLayer)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(dragLayer, screenPosition, null, out Vector3 worldPoint))
        {
            rectTransform.position = worldPoint;
        }
    }

    public void PlaceAt(RectTransform parent, Vector2 anchoredPosition)
    {
        transform.SetParent(parent, false);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = anchoredPosition;
        SetTilt(0f);
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        owner.TryPickUp(this);
    }
}
