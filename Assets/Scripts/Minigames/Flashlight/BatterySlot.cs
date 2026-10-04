using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class BatterySlot : MonoBehaviour
{
    [SerializeField] private bool plusFacesUp = true;

    public bool PlusFacesUp => plusFacesUp;

    public BatteryView Battery { get; set; }

    public RectTransform Rect => (RectTransform)transform;
}
