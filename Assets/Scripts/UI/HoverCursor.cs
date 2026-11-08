using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverCursor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Graphic cursor;
    [SerializeField] private float blinkSeconds = 0.4f;

    private bool isHovered;

    private void OnEnable()
    {
        isHovered = false;
        cursor.enabled = false;
    }

    private void Update()
    {
        if (!isHovered) return;

        cursor.enabled = Mathf.Repeat(Time.unscaledTime, blinkSeconds * 2f) < blinkSeconds;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        cursor.enabled = false;
    }
}
