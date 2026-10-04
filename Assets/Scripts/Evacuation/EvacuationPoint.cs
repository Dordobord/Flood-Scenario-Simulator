using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EvacuationPoint : MonoBehaviour
{
    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void Awake()
    {
        if (!GetComponent<Collider>().isTrigger)
        {
            Debug.LogWarning("EvacuationPoint: tick Is Trigger on the collider, or the player will bump into it instead of finishing.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<FirstPersonController>() == null) return;
        if (PhaseManager.Instance == null) return;

        PhaseManager.Instance.ReachEvacuationPoint();
    }
}
