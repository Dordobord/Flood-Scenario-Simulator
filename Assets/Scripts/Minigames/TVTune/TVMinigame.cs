using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TVMinigame : MinigameBase
{
    [Header("References")]
    [SerializeField] private Button upButton;
    [SerializeField] private Button downButton;
    [SerializeField] private TMP_Text channelText;
    [SerializeField] private Image[] signalBars;
    [SerializeField] private RawImage staticImage;
    [SerializeField] private GameObject picture;
    [SerializeField] private Image banner;
    [SerializeField] private TMP_Text bannerText;
    [SerializeField] private TMP_Text messageText;

    [Header("Channels")]
    [SerializeField] private int minChannel = 2;
    [SerializeField] private int maxChannel = 13;
    [SerializeField, Min(1)] private int minimumStartDistance = 3;
    [SerializeField] private string channelFormat = "00";

    [Header("Signal")]
    [SerializeField] private Color barOnColor = new Color(0.247f, 0.682f, 0.353f);
    [SerializeField] private Color barOffColor = new Color(0.275f, 0.275f, 0.275f);

    [Header("Static")]
    [SerializeField, Range(0f, 1f)] private float maxStaticAlpha = 1f;
    [SerializeField] private float staticJitter = 1f;

    [Header("Reveal")]
    [SerializeField] private float revealSeconds = 2.5f;

    private int channel;
    private int targetChannel;
    private bool isRevealed;
    private float revealTimer;

    private void Awake()
    {
        upButton.onClick.AddListener(() => ChangeChannel(1));
        downButton.onClick.AddListener(() => ChangeChannel(-1));
    }

    protected override void Setup()
    {
        isRevealed = false;
        revealTimer = 0f;

        targetChannel = Random.Range(minChannel, maxChannel + 1);
        channel = PickStartChannel();

        SetButtons(true);
        ShowWarning();
        UpdateScreen();
        AudioManager.PlayLoop(SoundLibrary.Current.tvStatic);
    }

    public override void End()
    {
        AudioManager.StopLoop(SoundLibrary.Current.tvStatic);
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

        if (staticImage != null) staticImage.uvRect = new Rect(Random.value * staticJitter, Random.value * staticJitter, 1f, 1f);
    }

    private int PickStartChannel()
    {
        int range = maxChannel - minChannel + 1;
        int distance = Mathf.Min(minimumStartDistance, range / 2);
        int offset = Random.Range(distance, range - distance + 1);
        return minChannel + (targetChannel - minChannel + offset) % range;
    }

    private void ChangeChannel(int step)
    {
        if (IsFinished || isRevealed) return;

        int range = maxChannel - minChannel + 1;
        channel = minChannel + ((channel - minChannel + step) % range + range) % range;
        UpdateScreen();
        AudioManager.PlaySfx(SoundLibrary.Current.tvClick);

        if (channel != targetChannel) return;

        isRevealed = true;
        SetButtons(false);
        AudioManager.StopLoop(SoundLibrary.Current.tvStatic);
    }

    private void UpdateScreen()
    {
        if (channelText != null) channelText.text = channel.ToString(channelFormat);

        int distance = GetDistance();
        int litBars = Mathf.Max(0, signalBars.Length - distance);
        for (int i = 0; i < signalBars.Length; i++)
        {
            signalBars[i].color = i < litBars ? barOnColor : barOffColor;
        }

        float noise = Mathf.Clamp01((float)distance / signalBars.Length);
        if (staticImage != null) SetAlpha(staticImage, noise * maxStaticAlpha);
        if (picture != null) picture.SetActive(distance == 0);
    }

    private int GetDistance()
    {
        int range = maxChannel - minChannel + 1;
        int distance = Mathf.Abs(channel - targetChannel);
        return Mathf.Min(distance, range - distance);
    }

    private void ShowWarning()
    {
        if (messageText != null) messageText.text = AddCenterName(settings.revealMessage);

        if (!settings.showRainfallWarning || TaskManager.Instance == null || TaskManager.Instance.CurrentScenario == null) return;

        GameSettings gameSettings = GameSettings.Current;
        RainfallWarningInfo warning = gameSettings.GetRainfallWarning(TaskManager.Instance.CurrentScenario.rainfallWarning);
        if (warning == null)
        {
            Debug.LogWarning("TVMinigame: no Rainfall Warning in GameSettings matches " + TaskManager.Instance.CurrentScenario.rainfallWarning + ".");
            return;
        }

        if (banner != null) banner.color = warning.color;
        if (bannerText != null) bannerText.text = string.Format(gameSettings.rainfallWarningTitleFormat, warning.label);
        if (messageText != null) messageText.text = warning.action;
    }

    private string AddCenterName(string message)
    {
        if (string.IsNullOrEmpty(message) || TaskManager.Instance == null || TaskManager.Instance.CurrentScenario == null) return message;
        return message.Replace("{0}", TaskManager.Instance.CurrentScenario.evacuationCenterName);
    }

    private void SetButtons(bool interactable)
    {
        upButton.interactable = interactable;
        downButton.interactable = interactable;
    }

    private static void SetAlpha(Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }
}
