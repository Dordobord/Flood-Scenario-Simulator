using UnityEngine;

public class PlayerFlashlight : MonoBehaviour
{
    [Header("Flashlight")]
    [SerializeField] private Light flashlight;
    [SerializeField] private GameObject heldModel;
    [SerializeField] private TaskData unlockTask;
    [SerializeField] private ItemData flashlightItem;

    private bool isUnlocked;
    private bool hintShown;

    private void Start()
    {
        SetFlashlightOn(false);

        if (TaskManager.Instance != null) TaskManager.Instance.TaskCompleted += OnTaskCompleted;
        if (InventoryManager.Instance != null) InventoryManager.Instance.SelectionChanged += OnSelectionChanged;
    }

    private void OnDestroy()
    {
        if (TaskManager.Instance != null) TaskManager.Instance.TaskCompleted -= OnTaskCompleted;
        if (InventoryManager.Instance != null) InventoryManager.Instance.SelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        if (!IsHoldingFlashlight()) SetFlashlightOn(false);
    }

    private bool IsHoldingFlashlight()
    {
        if (flashlightItem == null) return true;
        return InventoryManager.Instance != null && InventoryManager.Instance.IsHolding(flashlightItem);
    }

    private void Update()
    {
        if (!isUnlocked || !CanToggle()) return;

        if (MinigameInput.KeyPressedThisFrame(GameSettings.Current.flashlightKey))
        {
            SetFlashlightOn(!flashlight.enabled);

            SoundLibrary sounds = SoundLibrary.Current;
            AudioManager.PlaySfx(flashlight.enabled ? sounds.flashlightOn : sounds.flashlightOff);
        }
    }

    private void SetFlashlightOn(bool on)
    {
        flashlight.enabled = on;
        if (heldModel != null) heldModel.SetActive(on);
    }

    private bool CanToggle()
    {
        return IsHoldingFlashlight() && !MinigameManager.IsActive && !PauseMenuUI.IsPaused && !PhaseManager.IsPlayerLocked;
    }

    private void OnTaskCompleted(TaskData task)
    {
        if (task != unlockTask) return;

        isUnlocked = true;
        if (!hintShown) Invoke(nameof(ShowHintOnce), GameSettings.Current.flashlightHintDelay);
    }

    private void ShowHintOnce()
    {
        if (hintShown) return;

        hintShown = true;
        GameSettings settings = GameSettings.Current;
        GameEvents.RaiseMessage(string.Format(settings.flashlightReadyMessageFormat, settings.flashlightKey));
    }
}
