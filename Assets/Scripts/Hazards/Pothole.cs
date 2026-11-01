using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Pothole : MonoBehaviour
{
    [Header("Unlock")]
    [SerializeField] private TaskData hazardMapTask;

    [Header("Sign")]
    [SerializeField] private Transform sign;
    [SerializeField] private SpriteRenderer signSprite;
    [SerializeField] private float showDistance = 30f;

    private FirstPersonController playerInside;
    private Vector3 signStartPosition;
    private float bobPhase;

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

        bobPhase = transform.position.x + transform.position.z;

        if (sign == null) return;
        signStartPosition = sign.localPosition;
        sign.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (sign == null) return;

        bool evacuating = PhaseManager.Instance != null && PhaseManager.Instance.CurrentPhase == GamePhase.Evacuation;
        bool mapRead = TaskManager.Instance != null && TaskManager.Instance.IsCompleted(hazardMapTask);
        bool show = evacuating && mapRead && PlayerIsNear();

        if (sign.gameObject.activeSelf != show) sign.gameObject.SetActive(show);
        if (show) BobSign();
    }

    private bool PlayerIsNear()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return false;

        return (mainCamera.transform.position - transform.position).sqrMagnitude <= showDistance * showDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, showDistance);
    }

    private void BobSign()
    {
        GameSettings settings = GameSettings.Current;
        float offset = Mathf.Sin(Time.time * settings.hazardSignBobSpeed + bobPhase) * settings.hazardSignBobHeight;
        sign.localPosition = signStartPosition + Vector3.up * offset;

        if (signSprite != null) signSprite.color = settings.hazardSignColor;
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
        AudioManager.PlaySfx(SoundLibrary.Current.potholeSplash);
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerInside == null) return;
        if (other.GetComponentInParent<FirstPersonController>() != playerInside) return;

        playerInside.SpeedMultiplier = 1f;
        playerInside.CameraDrop = 0f;
        playerInside = null;
    }
}
