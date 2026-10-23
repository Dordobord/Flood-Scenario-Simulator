using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pothole : MonoBehaviour
{
    [Header("Unlock")]
    [SerializeField] private TaskData hazardMapTask;

    [Header("Visuals")]
    [SerializeField] private GameObject swirl;
    [SerializeField] private GameObject highlight;

    private FirstPersonController playerInside;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Awake()
    {
        if (!GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("Pothole: tick Is Trigger on the collider, or the player will bump into it instead of slowing down.");
        }

        SetVisible(swirl, false);
        SetVisible(highlight, false);
    }

    private void LateUpdate()
    {
        bool evacuating = PhaseManager.Instance != null && PhaseManager.Instance.CurrentPhase == GamePhase.Evacuation;
        bool mapRead = TaskManager.Instance != null && TaskManager.Instance.IsCompleted(hazardMapTask);

        SetVisible(swirl, evacuating && !mapRead);
        SetVisible(highlight, evacuating && mapRead);
    }

    private void OnTriggerEnter(Collider other)
    {
        FirstPersonController player = other.GetComponentInParent<FirstPersonController>();
        if (player == null || PhaseManager.Instance == null) return;
        if (PhaseManager.Instance.CurrentPhase != GamePhase.Evacuation) return;

        playerInside = player;
        player.SpeedMultiplier = GameSettings.Current.potholeSpeedMultiplier;
        player.CameraDrop = GameSettings.Current.potholeDepth;
        PhaseManager.Instance.AddPotholeStep();
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerInside == null) return;
        if (other.GetComponentInParent<FirstPersonController>() != playerInside) return;

        playerInside.SpeedMultiplier = 1f;
        playerInside.CameraDrop = 0f;
        playerInside = null;
    }

    private void SetVisible(GameObject target, bool show)
    {
        if (target != null && target.activeSelf != show) target.SetActive(show);
    }
}
