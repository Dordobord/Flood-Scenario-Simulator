using UnityEngine;

public class PrepBoundary : MonoBehaviour
{
    private static float lastMessageTime = -100f;

    private void Awake()
    {
        GameEvents.PreparationEnded += TurnOff;
    }

    private void OnDestroy()
    {
        GameEvents.PreparationEnded -= TurnOff;
    }

    private void TurnOff()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<FirstPersonController>() == null) return;

        GameSettings settings = GameSettings.Current;
        if (Time.time - lastMessageTime < settings.leaveHouseMessageCooldown) return;

        lastMessageTime = Time.time;
        GameEvents.RaiseMessage(settings.leaveHouseMessage);
    }
}
