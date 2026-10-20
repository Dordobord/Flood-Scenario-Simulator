using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HoldProgressMinigame : MinigameBase
{
    [SerializeField] private Image fillBar;
    [SerializeField] private TMP_Text hintText;

    [Header("HUD Use")]
    [SerializeField] private bool useInteractKey;

    private float progress;

    public float Progress
    {
        get { return progress; }
    }

    protected override void Setup()
    {
        progress = 0f;
        fillBar.fillAmount = 0f;

        if (hintText != null)
        {
            GameSettings gameSettings = GameSettings.Current;
            string keyName = MinigameInput.ActionName;
            if (useInteractKey)
            {
                keyName = gameSettings.interactKey.ToString();
            }
            hintText.text = string.Format(gameSettings.holdHintFormat, keyName);
        }
    }

    public override void Tick()
    {
        if (IsHeld())
        {
            progress += FillPerSecond() * Time.deltaTime;
        }
        else
        {
            progress -= FillPerSecond() * GameSettings.Current.holdDrainMultiplier * Time.deltaTime;
        }

        UpdateBar();
    }

    public void Drain()
    {
        progress -= FillPerSecond() * GameSettings.Current.holdDrainMultiplier * Time.deltaTime;
        UpdateBar();
    }

    private bool IsHeld()
    {
        if (useInteractKey) return MinigameInput.KeyHeld(GameSettings.Current.interactKey);
        return MinigameInput.ActionHeld;
    }

    private float FillPerSecond()
    {
        return 1f / Mathf.Max(0.1f, settings.holdSeconds);
    }

    private void UpdateBar()
    {
        progress = Mathf.Clamp01(progress);
        fillBar.fillAmount = progress;

        if (progress >= 1f) Finish(true);
    }
}
