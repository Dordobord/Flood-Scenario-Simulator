using UnityEngine;

public class ClosableWindow : MonoBehaviour, IInteractable
{
    [Header("Window")]
    [SerializeField] private Animator animator;
    [SerializeField] private string closedParameter = "IsClosed";
    [SerializeField] private TaskStepGroup group;

    private bool isClosed;

    public string Prompt
    {
        get
        {
            GameSettings settings = GameSettings.Current;
            if (isClosed) return settings.windowOpenPrompt;
            return settings.windowClosePrompt;
        }
    }

    public bool CanInteract
    {
        get { return group != null && !group.IsTaskDone; }
    }

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (group == null) group = GetComponentInParent<TaskStepGroup>();

        if (group != null) group.Register();
        else Debug.LogWarning("ClosableWindow: " + name + " is not under a Windows group, so it cannot be used.");
    }

    public void Interact()
    {
        if (!CanInteract) return;

        isClosed = !isClosed;
        if (animator != null) animator.SetBool(closedParameter, isClosed);

        if (!isClosed)
        {
            group.UndoStep();
            return;
        }

        GameSettings settings = GameSettings.Current;
        group.CompleteStep(settings.windowClosedMessageFormat, settings.windowName);
    }
}
