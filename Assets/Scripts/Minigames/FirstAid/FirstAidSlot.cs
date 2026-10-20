using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FirstAidSlot : MonoBehaviour, IDropHandler
{
    [Header("Slot")]
    [SerializeField] private FirstAidItem item;

    [Header("Parts")]
    [SerializeField] private Image silhouette;

    public event Action Filled;

    public FirstAidItem Item
    {
        get { return item; }
    }

    public bool IsFilled { get; private set; }

    private void OnValidate()
    {
        RefreshLater();
    }

    public void RefreshLater()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += RefreshIfAlive;
#endif
    }

    private void RefreshIfAlive()
    {
        if (this == null) return;
        ShowSilhouette();
    }

    public void ShowSilhouette()
    {
        if (item == null || item.Image == null || silhouette == null) return;

        silhouette.sprite = item.Image.sprite;
        silhouette.type = item.Image.type;
        ((RectTransform)transform).sizeDelta = ((RectTransform)item.transform).sizeDelta;
    }

    public void ResetSlot()
    {
        IsFilled = false;
        ShowSilhouette();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (IsFilled || eventData.pointerDrag == null) return;

        FirstAidItem dropped = eventData.pointerDrag.GetComponent<FirstAidItem>();
        if (dropped == null || dropped != item) return;

        dropped.PlaceIn((RectTransform)transform);
        IsFilled = true;
        if (Filled != null) Filled();
    }
}
