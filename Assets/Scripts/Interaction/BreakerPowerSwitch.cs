using UnityEngine;

public class BreakerPowerSwitch : MonoBehaviour, IInteractable
{
    public string Prompt
    {
        get { return GameSettings.Current.powerOnPrompt; }
    }

    public bool CanInteract
    {
        get { return PowerSupply.Instance != null && !PowerSupply.HasPower; }
    }

    public void Interact()
    {
        if (!CanInteract) return;
        PowerSupply.Instance.TurnPowerBackOn();
    }
}
