using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvacuationMarker : MonoBehaviour
{
    [Header("Unlock")]
    [SerializeField] private TaskData unlockTask;

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
        label.text = string.Format(settings.evacuationMarkerLabelFormat, GetCenterName(), Mathf.RoundToInt(meters));
        iconImage.color = settings.evacuationMarkerColor;
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

    private string GetCenterName()
    {
        if (TaskManager.Instance == null || TaskManager.Instance.CurrentScenario == null) return "";
        return TaskManager.Instance.CurrentScenario.evacuationCenterName;
    }

    private bool ShouldShow()
    {
        if (Camera.main == null || unlockTask == null) return false;
        if (PhaseManager.Instance == null || PhaseManager.Instance.CurrentPhase != GamePhase.Evacuation) return false;
        return TaskManager.Instance != null && TaskManager.Instance.IsCompleted(unlockTask);
    }
}
