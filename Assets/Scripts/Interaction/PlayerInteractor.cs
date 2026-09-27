using UnityEngine;

namespace StormWaits
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private LayerMask interactMask = ~0;

        private void Update()
        {
            
            if (MinigameManager.IsActive || PhaseManager.IsTransitionActive)
            {
                SetPrompt("");
                return;
            }

            GameSettings settings = GameSettings.Current;
            IInteractable target = FindTarget(settings.interactRange);

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

        private IInteractable FindTarget(float range)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, range, interactMask, QueryTriggerInteraction.Ignore))
            {
                return hit.collider.GetComponentInParent<IInteractable>();
            }
            return null;
        }

        private void SetPrompt(string text)
        {
            if (HudUI.Instance != null) HudUI.Instance.SetPrompt(text);
        }
    }
}
