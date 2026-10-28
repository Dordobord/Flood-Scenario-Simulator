using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FloodWarningMarker : MonoBehaviour
{
    [Header("Unlock")]
    [SerializeField] private TaskData hazardMapTask;

    [Header("Marker")]
    [SerializeField] private GameObject visual;
    [SerializeField] private RectTransform pin;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text label;

    [Header("Screen")]
    [SerializeField] private float edgePadding = 20f;

    private void Awake()
    {
        visual.SetActive(false);
    }

    private void LateUpdate()
    {
        bool show = ShouldShow();
        if (visual.activeSelf != show) visual.SetActive(show);
        if (!show) return;

        GameSettings settings = GameSettings.Current;
        Camera mainCamera = Camera.main;
        float meters = Vector3.Distance(mainCamera.transform.position, transform.position);

        pin.position = GetScreenPoint(mainCamera);
        label.text = string.Format(settings.floodWarningMarkerLabelFormat, Mathf.RoundToInt(meters));
        label.color = settings.floodWarningMarkerColor;
        iconImage.color = settings.floodWarningMarkerColor;
    }

    private Vector3 GetScreenPoint(Camera mainCamera)
    {
        Vector3 point = mainCamera.WorldToScreenPoint(transform.position);

        Vector2 size = Vector2.Scale(pin.rect.size, pin.lossyScale);
        float minX = size.x * pin.pivot.x + edgePadding;
        float maxX = Screen.width - size.x * (1f - pin.pivot.x) - edgePadding;
        float minY = size.y * pin.pivot.y + edgePadding;
        float maxY = Screen.height - size.y * (1f - pin.pivot.y) - edgePadding;

        if (point.z < 0f)
        {
            point.x = Screen.width - point.x;
            point.y = minY;
        }

        point.x = Mathf.Clamp(point.x, minX, maxX);
        point.y = Mathf.Clamp(point.y, minY, maxY);
        point.z = 0f;
        return point;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.96f, 0.77f, 0.09f);
        Gizmos.DrawWireSphere(transform.position, 1f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 3f);
    }

    private bool ShouldShow()
    {
        if (Camera.main == null || hazardMapTask == null) return false;
        if (PhaseManager.Instance == null || PhaseManager.Instance.CurrentPhase != GamePhase.Evacuation) return false;
        return TaskManager.Instance != null && TaskManager.Instance.IsCompleted(hazardMapTask);
    }
}
