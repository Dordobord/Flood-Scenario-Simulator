using UnityEngine;

public class HouseDoor : MonoBehaviour, IInteractable
{
    [Header("Door")]
    [SerializeField] private Transform hinge;
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float swingSpeed = 240f;
    [SerializeField] private bool startOpen;

    private bool isOpen;
    private Quaternion closedRotation;

    public string Prompt
    {
        get
        {
            GameSettings settings = GameSettings.Current;
            if (isOpen) return settings.doorClosePrompt;
            return settings.doorOpenPrompt;
        }
    }

    public bool CanInteract
    {
        get { return hinge != null; }
    }

    private void Awake()
    {
        if (hinge == null) hinge = transform;
        closedRotation = hinge.localRotation;
        isOpen = startOpen;
        if (isOpen) hinge.localRotation = TargetRotation();
    }

    public void Interact()
    {
        if (!CanInteract) return;
        isOpen = !isOpen;
    }

    private void Update()
    {
        if (hinge == null) return;
        hinge.localRotation = Quaternion.RotateTowards(hinge.localRotation, TargetRotation(), swingSpeed * Time.deltaTime);
    }

    private Quaternion TargetRotation()
    {
        if (!isOpen) return closedRotation;
        return closedRotation * Quaternion.Euler(0f, openAngle, 0f);
    }
}
