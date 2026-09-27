using TMPro;
using UnityEngine;

namespace StormWaits
{
    
    public class TimerUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text timerText;

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
                timerText.text = settings.floodTimerText;
                timerText.color = settings.timerWarningColor;
                return;
            }

            float remaining = Mathf.Max(0f, PhaseManager.Instance.TimeRemaining);
            int minutes = Mathf.FloorToInt(remaining / 60f);
            int seconds = Mathf.FloorToInt(remaining % 60f);
            string clock = minutes.ToString("00") + ":" + seconds.ToString("00");

            timerText.text = string.Format(settings.prepTimerFormat, clock);
            timerText.color = remaining <= settings.timerWarningSeconds ? settings.timerWarningColor : settings.timerNormalColor;
        }
    }
}
