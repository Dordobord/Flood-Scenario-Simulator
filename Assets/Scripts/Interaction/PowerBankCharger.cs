using TMPro;
using UnityEngine;

public class PowerBankCharger : MonoBehaviour, IInteractable
{
    [Header("Task")]
    [SerializeField] private TaskData task;

    [Header("Charging")]
    [SerializeField] private float chargeSeconds = 30f;

    [Header("Label")]
    [SerializeField] private TMP_Text label;

    private bool pluggedIn;
    private float charge;
    private float plugTime;

    public string Prompt
    {
        get
        {
            GameSettings settings = GameSettings.Current;
            if (IsFull()) return settings.powerBankTakePrompt;
            if (!PowerSupply.HasPower) return settings.noPowerPrompt;
            if (pluggedIn) return string.Format(settings.powerBankChargingFormat, Mathf.FloorToInt(charge * 100f));
            return settings.powerBankPlugPrompt;
        }
    }

    public bool CanInteract
    {
        get { return task != null && (TaskManager.Instance == null || !TaskManager.Instance.IsCompleted(task)); }
    }

    private void Update()
    {
        if (pluggedIn && PowerSupply.HasPower && !IsFull())
        {
            charge = Mathf.Min(1f, charge + Time.deltaTime / Mathf.Max(0.01f, chargeSeconds));
        }

        if (label != null && pluggedIn)
        {
            label.text = string.Format(GameSettings.Current.powerBankLabelFormat, Mathf.FloorToInt(charge * 100f));
        }
    }

    public void Interact()
    {
        if (!CanInteract) return;
        GameSettings settings = GameSettings.Current;

        if (IsFull())
        {
            if (TaskManager.Instance != null) TaskManager.Instance.RecordAttempt(task, true, Time.time - plugTime);
            GameEvents.RaiseMessage(settings.powerBankPackedMessage);
            gameObject.SetActive(false);
            return;
        }

        if (!PowerSupply.HasPower)
        {
            GameEvents.RaiseMessage(settings.noPowerMessage);
            return;
        }

        if (pluggedIn)
        {
            GameEvents.RaiseMessage(settings.powerBankStillChargingMessage);
            return;
        }

        pluggedIn = true;
        plugTime = Time.time;
    }

    private bool IsFull()
    {
        return charge >= 1f;
    }
}
