using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private LayerMask interactMask = ~0;

    private IHoldInteractable currentHold;

    private void Update()
    {
        if (MinigameManager.IsActive || PhaseManager.IsPlayerLocked)
        {
            SetHoldTarget(null);
            SetPrompt("");
            return;
        }

        GameSettings settings = GameSettings.Current;
        Collider hitCollider = FindCollider(settings.interactRange);

        IHoldInteractable holdTarget = null;
        if (hitCollider != null) holdTarget = hitCollider.GetComponentInParent<IHoldInteractable>();
        if (holdTarget != null && !holdTarget.CanHold) holdTarget = null;

        SetHoldTarget(holdTarget);

        if (currentHold != null)
        {
            IInteractable holdInfo = hitCollider.GetComponentInParent<IInteractable>();
            if (holdInfo != null)
            {
                SetPrompt(string.Format(settings.holdPromptFormat, settings.interactKey, holdInfo.Prompt));
            }
            else
            {
                SetPrompt("");
            }
            currentHold.HoldTick();
            return;
        }

        IInteractable target = null;
        if (hitCollider != null) target = FindInteractable(hitCollider);

        if (target == null || !target.CanInteract)
        {
            SetPrompt("");
            return;
        }

        SetPrompt(string.Format(settings.promptFormat, settings.interactKey, target.Prompt));

        if (MinigameInput.KeyPressedThisFrame(settings.interactKey))
        {
            target.Interact();
        }
    }

    private void OnDisable()
    {
        SetHoldTarget(null);
    }

    private IInteractable FindInteractable(Collider hitCollider)
    {
        IInteractable[] interactables = hitCollider.GetComponentsInParent<IInteractable>();
        foreach (IInteractable interactable in interactables)
        {
            if (interactable.CanInteract) return interactable;
        }
        return null;
    }

    private Collider FindCollider(float range)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, range, interactMask, QueryTriggerInteraction.Ignore))
        {
            return hit.collider;
        }
        return null;
    }

    private void SetHoldTarget(IHoldInteractable target)
    {
        if (currentHold == target) return;

        if (currentHold != null) currentHold.HoldCancel();
        currentHold = target;
    }

    private void SetPrompt(string text)
    {
        if (HudUI.Instance != null) HudUI.Instance.SetPrompt(text);
    }
}
