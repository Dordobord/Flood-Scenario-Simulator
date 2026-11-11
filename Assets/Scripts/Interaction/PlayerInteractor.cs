using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [SerializeField] private LayerMask interactMask = ~0;

    private IHoldInteractable currentHold;
    private HoverOutline currentOutline;

    private void Update()
    {
        if (MinigameManager.IsActive || PhaseManager.IsPlayerLocked)
        {
            SetHoldTarget(null);
            SetOutline(null);
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
            SetOutline(OutlineFor(currentHold as Component));
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
            SetOutline(null);
            SetPrompt("");
            return;
        }

        SetOutline(OutlineFor(target as Component));
        SetPrompt(string.Format(settings.promptFormat, settings.interactKey, target.Prompt));

        if (MinigameInput.KeyPressedThisFrame(settings.interactKey))
        {
            target.Interact();
        }
    }

    private void OnDisable()
    {
        SetHoldTarget(null);
        SetOutline(null);
    }

    private HoverOutline OutlineFor(Component owner)
    {
        if (owner == null) return null;

        HoverOutline outline = owner.GetComponentInParent<HoverOutline>();
        if (outline == null) outline = owner.gameObject.AddComponent<HoverOutline>();
        return outline;
    }

    private void SetOutline(HoverOutline outline)
    {
        if (currentOutline == outline) return;

        if (currentOutline != null) currentOutline.SetShown(false);
        currentOutline = outline;
        if (currentOutline != null) currentOutline.SetShown(true);
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
