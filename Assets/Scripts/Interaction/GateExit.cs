using UnityEngine;

public class GateExit : MonoBehaviour, IInteractable
{
    private float confirmUntil = -1f;

    private bool IsConfirming
    {
        get { return Time.time < confirmUntil; }
    }

    public string Prompt
    {
        get
        {
            GameSettings settings = GameSettings.Current;
            if (IsConfirming) return settings.leaveConfirmPrompt;
            return settings.leavePrompt;
        }
    }

    public bool CanInteract
    {
        get { return PhaseManager.Instance != null && PhaseManager.Instance.CurrentPhase == GamePhase.Preparation; }
    }

    public void Interact()
    {
        GameSettings settings = GameSettings.Current;

        if (!IsConfirming)
        {
            confirmUntil = Time.time + settings.leaveConfirmSeconds;
            GameEvents.RaiseMessage(string.Format(settings.leaveConfirmMessageFormat, settings.interactKey));
            return;
        }

        confirmUntil = -1f;
        PhaseManager.Instance.LeaveEarly();
    }
}
