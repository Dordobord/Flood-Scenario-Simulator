using TMPro;
using UnityEngine;

public class AppliancePlug : MonoBehaviour, IInteractable
{
    [Header("Appliance")]
    [SerializeField] private string applianceName = "Appliance";
    [SerializeField] private TaskStepGroup group;

    [Header("Label")]
    [SerializeField] private TMP_Text label;

    private bool unplugged;

    public string ApplianceName
    {
        get { return applianceName; }
    }

    public bool IsPluggedIn
    {
        get { return !unplugged; }
    }

    private void Awake()
    {
        if (group == null) group = GetComponentInParent<TaskStepGroup>();
        if (group != null) group.Register();
    }

    public string Prompt
    {
        get
        {
            if (!PowerSupply.HasPower) return GameSettings.Current.noPowerPrompt;
            return string.Format(GameSettings.Current.unplugPromptFormat, applianceName);
        }
    }

    public bool CanInteract
    {
        get { return !unplugged && group != null && !group.IsTaskDone; }
    }

    public void Interact()
    {
        if (!CanInteract) return;

        if (!PowerSupply.HasPower)
        {
            GameEvents.RaiseMessage(GameSettings.Current.noPowerMessage);
            return;
        }

        unplugged = true;
        if (label != null) label.text = string.Format(GameSettings.Current.unpluggedLabelFormat, applianceName);
        group.CompleteStep(GameSettings.Current.unpluggedMessageFormat, applianceName);
    }
}
