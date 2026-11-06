using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color warningColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private Color evacuationColor = new Color(1f, 0.3f, 0.3f);

    private int lastBeepSecond = -1;

    private void Update()
    {
        if (PhaseManager.Instance == null)
        {
            timerText.text = "";
            return;
        }

        GameSettings settings = GameSettings.Current;

        if (PhaseManager.Instance.CurrentPhase != GamePhase.Preparation)
        {
            timerText.text = settings.evacuationTimerText;
            timerText.color = evacuationColor;
            return;
        }

        float remaining = Mathf.Max(0f, PhaseManager.Instance.TimeRemaining);
        int minutes = Mathf.FloorToInt(remaining / 60f);
        int seconds = Mathf.FloorToInt(remaining % 60f);
        string clock = minutes.ToString("00") + ":" + seconds.ToString("00");

        timerText.text = string.Format(settings.prepTimerFormat, clock);
        timerText.color = remaining <= settings.timerWarningSeconds ? warningColor : normalColor;

        BeepEachSecond(remaining, settings);
    }

    private void BeepEachSecond(float remaining, GameSettings settings)
    {
        if (remaining <= 0f || remaining > settings.timerWarningSeconds) return;

        int second = Mathf.CeilToInt(remaining);
        if (second == lastBeepSecond) return;

        lastBeepSecond = second;
        AudioManager.PlaySfx(SoundLibrary.Current.timerBeep);
    }
}
