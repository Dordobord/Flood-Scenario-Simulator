using UnityEngine;
using UnityEngine.InputSystem;

public abstract class HoldInteractable : MonoBehaviour, IHoldInteractable
{
    private HoldProgressMinigame activeBar;
    private float holdStartTime;
    private bool isBeingHeld;

    public bool CanHold
    {
        get { return CanStartHold() && GetHudBar() != null; }
    }

    protected abstract MinigameSettings HoldSettings { get; }

    protected abstract bool CanStartHold();

    protected abstract void OnHoldComplete(float seconds);

    protected virtual bool CanBeginHold(bool justPressed)
    {
        return true;
    }

    protected virtual void OnEnable()
    {
        GameEvents.PreparationEnded += StopHold;
    }

    protected virtual void OnDisable()
    {
        GameEvents.PreparationEnded -= StopHold;
        StopHold();
    }

    private void Update()
    {
        if (activeBar == null || isBeingHeld) return;

        activeBar.Drain();
        if (activeBar.Progress <= 0f) StopHold();
    }

    public void HoldTick()
    {
        if (!CanHold)
        {
            StopHold();
            return;
        }

        isBeingHeld = true;
        Key interactKey = GameSettings.Current.interactKey;

        if (activeBar == null)
        {
            if (!MinigameInput.KeyHeld(interactKey)) return;
            if (!CanBeginHold(MinigameInput.KeyPressedThisFrame(interactKey))) return;

            StartHold();
        }

        activeBar.Tick();

        if (activeBar != null && activeBar.Progress <= 0f && !MinigameInput.KeyHeld(interactKey))
        {
            StopHold();
        }
    }

    public void HoldCancel()
    {
        isBeingHeld = false;
    }

    public void StopHold()
    {
        if (activeBar == null) return;

        activeBar.Finished -= OnBarFinished;
        activeBar.End();
        activeBar = null;

        if (HudUI.Instance != null && HudUI.Instance.HoldBarUser == this)
        {
            HudUI.Instance.HoldBarUser = null;
        }
    }

    private HoldProgressMinigame GetHudBar()
    {
        if (HudUI.Instance == null) return null;
        return HudUI.Instance.HoldBar;
    }

    private void StartHold()
    {
        HudUI hud = HudUI.Instance;

        if (hud.HoldBarUser != null && hud.HoldBarUser != this)
        {
            hud.HoldBarUser.StopHold();
        }
        hud.HoldBarUser = this;

        activeBar = hud.HoldBar;
        activeBar.Finished += OnBarFinished;
        activeBar.Begin(HoldSettings);
        holdStartTime = Time.time;
    }

    private void OnBarFinished(bool success)
    {
        float seconds = Time.time - holdStartTime;
        StopHold();

        if (success)
        {
            OnHoldComplete(seconds);
        }
    }
}
