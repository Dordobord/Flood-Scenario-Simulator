using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StoveMinigame : MinigameBase
{
    [Header("References")]
    [SerializeField] private StoveKnob[] knobs;
    [SerializeField] private RectTransform valve;
    [SerializeField] private Image valveGauge;

    [Header("Knobs")]
    [SerializeField] private float offAngle = 0f;
    [SerializeField] private float offZoneDegrees = 15f;
    [SerializeField, Range(0f, 180f)] private float minimumStartDistance = 90f;

    [Header("Valve")]
    [SerializeField] private float turnsToClose = 1.5f;

    [Header("Crank Sound")]
    [SerializeField] private float crankStopDelay = 0.1f;
    [SerializeField] private float crankMinDegrees = 0.5f;

    private StoveKnob draggedKnob;
    private float grabOffset;
    private bool draggingValve;
    private float lastValveAngle;
    private float valveProgress;
    private float crankTimer;

    protected override void Setup()
    {
        foreach (StoveKnob knob in knobs)
        {
            float startDistance = Random.Range(minimumStartDistance, 360f - minimumStartDistance);
            knob.Setup(offAngle + startDistance);
        }

        draggedKnob = null;
        draggingValve = false;
        valveProgress = 0f;
        crankTimer = 0f;
        UpdateValve();
    }

    public override void End()
    {
        StopCrankSound();
        base.End();
    }

    public override void Tick()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || !MinigameManager.CursorFree)
        {
            draggedKnob = null;
            draggingValve = false;
            StopCrankSound();
            return;
        }

        Vector2 mousePosition = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame) StartDrag(mousePosition);

        if (!mouse.leftButton.isPressed)
        {
            draggedKnob = null;
            draggingValve = false;
        }

        if (draggedKnob != null) DragKnob(mousePosition);
        if (draggingValve) DragValve(mousePosition);

        UpdateCrankSound();
    }

    private void StartDrag(Vector2 mousePosition)
    {
        foreach (StoveKnob knob in knobs)
        {
            if (knob.IsOff) continue;
            if (!RectTransformUtility.RectangleContainsScreenPoint(knob.Dial, mousePosition, null)) continue;

            draggedKnob = knob;
            grabOffset = Mathf.DeltaAngle(GetPointerAngle(knob.Dial, mousePosition), knob.Angle);
            return;
        }

        if (!RectTransformUtility.RectangleContainsScreenPoint(valve, mousePosition, null)) return;

        if (!AllKnobsOff())
        {
            ShowFeedback(GameSettings.Current.stoveValveLockedHint);
            return;
        }

        draggingValve = true;
        lastValveAngle = GetPointerAngle(valve, mousePosition);
    }

    private void DragKnob(Vector2 mousePosition)
    {
        float angle = GetPointerAngle(draggedKnob.Dial, mousePosition) + grabOffset;
        float turned = Mathf.Abs(Mathf.DeltaAngle(draggedKnob.Angle, angle));
        if (turned >= crankMinDegrees) crankTimer = crankStopDelay;

        draggedKnob.SetAngle(angle);

        if (Mathf.Abs(Mathf.DeltaAngle(angle, offAngle)) > offZoneDegrees) return;

        draggedKnob.TurnOff(offAngle);
        draggedKnob = null;
        crankTimer = 0f;

        if (AllKnobsOff()) ShowInstructions(GameSettings.Current.stoveValveHint);
    }

    private void DragValve(Vector2 mousePosition)
    {
        float angle = GetPointerAngle(valve, mousePosition);
        float delta = Mathf.DeltaAngle(lastValveAngle, angle);
        lastValveAngle = angle;

        float degreesToClose = Mathf.Max(1f, turnsToClose * 360f);
        valveProgress = Mathf.Clamp01(valveProgress - delta / degreesToClose);
        UpdateValve();

        if (valveProgress >= 1f)
        {
            draggingValve = false;
            Finish(true);
        }
    }

    private void UpdateValve()
    {
        float degreesToClose = turnsToClose * 360f;
        valve.localEulerAngles = new Vector3(0f, 0f, -valveProgress * degreesToClose);
        if (valveGauge != null) valveGauge.fillAmount = valveProgress;
    }

    private void UpdateCrankSound()
    {
        if (draggedKnob == null) crankTimer = 0f;

        if (crankTimer > 0f)
        {
            crankTimer -= Time.unscaledDeltaTime;
            AudioManager.PlayLoop(SoundLibrary.Current.stoveCrank);
        }
        else
        {
            StopCrankSound();
        }
    }

    private void StopCrankSound()
    {
        crankTimer = 0f;
        AudioManager.StopLoop(SoundLibrary.Current.stoveCrank);
    }

    private bool AllKnobsOff()
    {
        foreach (StoveKnob knob in knobs)
        {
            if (!knob.IsOff) return false;
        }
        return true;
    }

    private float GetPointerAngle(RectTransform target, Vector2 screenPosition)
    {
        Vector2 center = RectTransformUtility.WorldToScreenPoint(null, target.position);
        Vector2 direction = screenPosition - center;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }
}
