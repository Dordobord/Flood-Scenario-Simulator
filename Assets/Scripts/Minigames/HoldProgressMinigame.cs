using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StormWaits
{
    public class HoldProgressMinigame : MinigameBase
    {
        [SerializeField] private Image fillBar;
        [SerializeField] private TMP_Text hintText;

        private float progress;

        protected override void Setup()
        {
            progress = 0f;
            fillBar.fillAmount = 0f;

            if (hintText != null)
            {
                hintText.text = string.Format(GameSettings.Current.holdHintFormat, MinigameInput.ActionName);
            }
        }

        public override void Tick()
        {
            float fillPerSecond = 1f / Mathf.Max(0.1f, settings.holdSeconds);

            if (MinigameInput.ActionHeld)
            {
                progress += fillPerSecond * Time.deltaTime;
            }
            else
            {
                progress -= fillPerSecond * GameSettings.Current.holdDrainMultiplier * Time.deltaTime;
            }

            progress = Mathf.Clamp01(progress);
            fillBar.fillAmount = progress;

            if (progress >= 1f) Finish(true);
        }
    }
}
