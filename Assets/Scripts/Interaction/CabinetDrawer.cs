using UnityEngine;

public class CabinetDrawer : MonoBehaviour, IInteractable
{
    [Header("Drawer")]
    [SerializeField] private Animator animator;
    [SerializeField] private string openParameter = "IsOpen";

    private bool isOpen;

    public string Prompt
    {
        get
        {
            GameSettings settings = GameSettings.Current;
            if (isOpen) return settings.drawerClosePrompt;
            return settings.drawerOpenPrompt;
        }
    }

    public bool CanInteract
    {
        get { return animator != null; }
    }

    private void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void Interact()
    {
        if (!CanInteract) return;

        isOpen = !isOpen;
        animator.SetBool(openParameter, isOpen);

        SoundLibrary sounds = SoundLibrary.Current;
        AudioManager.PlaySfx(isOpen ? sounds.drawerOpen : sounds.drawerClose);
    }
}
