using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FloodedRoute : MonoBehaviour
{
    [Header("Unlock")]
    [SerializeField] private TaskData hazardMapTask;

    [Header("Visuals")]
    [SerializeField] private GameObject mapReadSign;
    [SerializeField] private GameObject deepWater;

    private float lastMessageTime = -999f;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Awake()
    {
        if (!GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("FloodedRoute: tick Is Trigger on the warning zone collider. The Barrier child is what blocks the road.");
        }

        SetVisible(mapReadSign, false);
        SetVisible(deepWater, false);
    }

    private void LateUpdate()
    {
        GamePhase phase = PhaseManager.Instance != null ? PhaseManager.Instance.CurrentPhase : GamePhase.Preparation;
        bool evacuating = phase == GamePhase.Evacuation;
        bool mapRead = TaskManager.Instance != null && TaskManager.Instance.IsCompleted(hazardMapTask);

        SetVisible(mapReadSign, evacuating && mapRead);
        SetVisible(deepWater, evacuating || phase == GamePhase.Finished);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<FirstPersonController>() == null || PhaseManager.Instance == null) return;
        if (PhaseManager.Instance.CurrentPhase != GamePhase.Evacuation) return;
        if (Time.time - lastMessageTime < GameSettings.Current.floodedRouteMessageCooldown) return;

        lastMessageTime = Time.time;
        GameEvents.RaiseMessage(GameSettings.Current.floodedRouteMessage);
    }

    private void SetVisible(GameObject target, bool show)
    {
        if (target != null && target.activeSelf != show) target.SetActive(show);
    }
}
