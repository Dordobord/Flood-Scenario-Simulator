using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RadioMinigame : MinigameBase
{
    [Header("References")]
    [SerializeField] private Slider tuningBar;
    [SerializeField] private RectTransform needle;
    [SerializeField] private RectTransform needleTrack;
    [SerializeField] private TMP_Text frequencyText;
    [SerializeField] private TMP_Text messageText;

    [Header("Band")]
    [SerializeField] private float minFrequency = 88f;
    [SerializeField] private float maxFrequency = 108f;
    [SerializeField] private string frequencyFormat = "{0:0.0} MHz";

    [Header("Tuning")]
    [SerializeField] private float clearRange = 1f;
    [SerializeField] private float minimumStartDistance = 5f;
    [SerializeField] private float holdSeconds = 0.8f;
    [SerializeField] private float revealSeconds = 2.5f;

    [Header("Static")]
    [SerializeField] private float garbleRange = 6f;
    [SerializeField] private string staticSymbols = "#.*-~";

    private float targetFrequency;
    private float timeInClearZone;
    private bool isRevealed;
    private float revealTimer;
    private string fullMessage;
    private bool hasTuned;

    public override bool CanReopen
    {
        get { return hasTuned; }
    }

    private void Awake()
    {
        tuningBar.onValueChanged.AddListener(OnFrequencyChanged);
    }

    protected override void Setup()
    {
        isRevealed = false;
        revealTimer = 0f;
        timeInClearZone = 0f;

        targetFrequency = Mathf.Round(Random.Range(minFrequency, maxFrequency) * 10f) / 10f;

        tuningBar.minValue = minFrequency;
        tuningBar.maxValue = maxFrequency;
        tuningBar.interactable = true;
        tuningBar.SetValueWithoutNotify(PickStartFrequency());

        fullMessage = AddCenterName(settings.revealMessage);
        OnFrequencyChanged(tuningBar.value);
        AudioManager.PlayLoop(SoundLibrary.Current.radioStatic);
    }

    public override void End()
    {
        AudioManager.StopLoop(SoundLibrary.Current.radioStatic);
        base.End();
    }

    public override void Tick()
    {
        if (isRevealed)
        {
            revealTimer += Time.deltaTime;
            if (revealTimer >= revealSeconds) Finish(true);
            return;
        }

        if (Mathf.Abs(tuningBar.value - targetFrequency) > clearRange)
        {
            timeInClearZone = 0f;
            return;
        }

        timeInClearZone += Time.deltaTime;
        if (timeInClearZone >= holdSeconds) Reveal();
    }

    private float PickStartFrequency()
    {
        float low = targetFrequency - minimumStartDistance;
        float high = targetFrequency + minimumStartDistance;

        bool canGoLow = low >= minFrequency;
        bool canGoHigh = high <= maxFrequency;

        if (canGoLow && (!canGoHigh || Random.value < 0.5f)) return Random.Range(minFrequency, low);
        if (canGoHigh) return Random.Range(high, maxFrequency);
        return minFrequency;
    }

    private void OnFrequencyChanged(float frequency)
    {
        if (frequencyText != null) frequencyText.text = string.Format(frequencyFormat, frequency);
        if (!isRevealed) ShowGarbled(frequency);

        if (needle == null || needleTrack == null) return;

        float t = Mathf.InverseLerp(minFrequency, maxFrequency, frequency);
        Vector3 left = needleTrack.TransformPoint(new Vector3(needleTrack.rect.xMin, 0f, 0f));
        Vector3 right = needleTrack.TransformPoint(new Vector3(needleTrack.rect.xMax, 0f, 0f));
        Vector3 position = needle.position;
        position.x = Mathf.Lerp(left.x, right.x, t);
        needle.position = position;
    }

    private void Reveal()
    {
        isRevealed = true;
        hasTuned = true;
        tuningBar.interactable = false;
        AudioManager.StopLoop(SoundLibrary.Current.radioStatic);

        if (messageText != null) messageText.text = fullMessage;
    }

    protected override void ShowFinished()
    {
        isRevealed = true;
        tuningBar.interactable = false;
        tuningBar.SetValueWithoutNotify(targetFrequency);
        fullMessage = AddCenterName(settings.revealMessage);
        OnFrequencyChanged(targetFrequency);
        if (messageText != null) messageText.text = fullMessage;
    }

    private void ShowGarbled(float frequency)
    {
        if (messageText == null || string.IsNullOrEmpty(fullMessage) || string.IsNullOrEmpty(staticSymbols)) return;

        float noise = Mathf.InverseLerp(clearRange, garbleRange, Mathf.Abs(frequency - targetFrequency));
        char[] letters = fullMessage.ToCharArray();
        for (int i = 0; i < letters.Length; i++)
        {
            if (char.IsWhiteSpace(letters[i])) continue;
            if (Random.value < noise) letters[i] = staticSymbols[Random.Range(0, staticSymbols.Length)];
        }
        messageText.text = new string(letters);
    }

    private string AddCenterName(string message)
    {
        if (string.IsNullOrEmpty(message) || TaskManager.Instance == null || TaskManager.Instance.CurrentScenario == null) return message;
        return message.Replace("{0}", TaskManager.Instance.CurrentScenario.evacuationCenterName);
    }
}
