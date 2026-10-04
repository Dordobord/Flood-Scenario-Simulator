using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TuningMinigame : MinigameBase
{
    [Header("References")]
    [SerializeField] private RectTransform knob;
    [SerializeField] private RawImage staticImage;
    [SerializeField] private Graphic screenPicture;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text hintText;

    [Header("Tuning")]
    [SerializeField] private float clearZoneDegrees = 12f;
    [SerializeField] private float staticFadeDegrees = 120f;
    [SerializeField] private float holdSeconds = 0.8f;
    [SerializeField] private float revealSeconds = 2.5f;
    [SerializeField] private bool randomizeClearAngle = true;
    [SerializeField] private float fixedClearAngle = 90f;
    [SerializeField, Range(0f, 180f)] private float minimumStartDistance = 90f;

    [Header("Static")]
    [SerializeField, Range(0f, 1f)] private float maxStaticAlpha = 1f;
    [SerializeField] private float staticJitter = 1f;

    [Header("Text")]
    [SerializeField] private string hintFormat = "Click and drag the knob in a circle to tune in.";

    private float knobAngle;
    private float clearAngle;
    private float grabOffset;
    private bool isDragging;
    private float timeInClearZone;
    private bool isRevealed;
    private float revealTimer;

    protected override void Setup()
    {
        isDragging = false;
        isRevealed = false;
        timeInClearZone = 0f;
        revealTimer = 0f;

        clearAngle = randomizeClearAngle ? Random.Range(0f, 360f) : fixedClearAngle;

        float startDistance = Random.Range(minimumStartDistance, 360f - minimumStartDistance);
        knobAngle = Mathf.Repeat(clearAngle + startDistance, 360f);
        ApplyKnobRotation();

        if (messageText != null) messageText.text = "";
        if (hintText != null) hintText.text = hintFormat;

        UpdateStatic(GetDistanceToClear());
    }

    public override void Tick()
    {
        if (isRevealed)
        {
            revealTimer += Time.deltaTime;
            if (revealTimer >= revealSeconds) Finish(true);
            return;
        }

        AnimateStatic();
        HandleDrag();

        float distance = GetDistanceToClear();
        UpdateStatic(distance);

        if (distance <= clearZoneDegrees)
        {
            timeInClearZone += Time.deltaTime;
            if (timeInClearZone >= holdSeconds) Reveal();
        }
        else
        {
            timeInClearZone = 0f;
        }
    }

    private void HandleDrag()
    {
        Mouse mouse = Mouse.current;
        if (mouse == null || !MinigameManager.CursorFree)
        {
            isDragging = false;
            return;
        }

        Vector2 mousePosition = mouse.position.ReadValue();

        if (mouse.leftButton.wasPressedThisFrame && RectTransformUtility.RectangleContainsScreenPoint(knob, mousePosition, null))
        {
            isDragging = true;
            grabOffset = Mathf.DeltaAngle(GetPointerAngle(mousePosition), knobAngle);
        }

        if (!mouse.leftButton.isPressed) isDragging = false;

        if (isDragging)
        {
            knobAngle = Mathf.Repeat(GetPointerAngle(mousePosition) + grabOffset, 360f);
            ApplyKnobRotation();
        }
    }

    private float GetPointerAngle(Vector2 screenPosition)
    {
        Vector2 knobCenter = RectTransformUtility.WorldToScreenPoint(null, knob.position);
        Vector2 direction = screenPosition - knobCenter;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private float GetDistanceToClear()
    {
        return Mathf.Abs(Mathf.DeltaAngle(knobAngle, clearAngle));
    }

    private void ApplyKnobRotation()
    {
        knob.localEulerAngles = new Vector3(0f, 0f, knobAngle);
    }

    private void UpdateStatic(float distance)
    {
        float noise = Mathf.InverseLerp(clearZoneDegrees, staticFadeDegrees, distance);

        if (staticImage != null) SetAlpha(staticImage, noise * maxStaticAlpha);
        if (screenPicture != null) SetAlpha(screenPicture, 1f - noise);
    }

    private void AnimateStatic()
    {
        if (staticImage == null) return;
        staticImage.uvRect = new Rect(Random.value * staticJitter, Random.value * staticJitter, 1f, 1f);
    }

    private void Reveal()
    {
        isRevealed = true;
        isDragging = false;
        UpdateStatic(0f);

        string message = settings.revealMessage;
        if (messageText != null) messageText.text = message;
        if (string.IsNullOrEmpty(message)) revealTimer = revealSeconds;
    }

    private static void SetAlpha(Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }
}
