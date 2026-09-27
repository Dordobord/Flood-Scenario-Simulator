using TMPro;
using UnityEngine;

namespace StormWaits
{
    public class TimingBarMinigame : MinigameBase
    {
        [SerializeField] private RectTransform targetZone;
        [SerializeField] private RectTransform cursor;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private float cursorWidth = 10f;

        private float position;       
        private float direction = 1f;
        private float targetStart;
        private float targetWidth;
        private int hits;
        private int requiredHits;

        protected override void Setup()
        {
            position = 0f;
            direction = 1f;
            hits = 0;
            targetWidth = Mathf.Clamp(settings.targetWidth, 0.05f, 0.5f);
            requiredHits = Mathf.Max(1, settings.requiredHits);

            
            if (hintText != null)
            {
                hintText.text = string.Format(GameSettings.Current.timingHintFormat, MinigameInput.ActionName);
            }

            PickTarget();
            MoveCursor();
        }

        public override void Tick()
        {
            
            position += direction * Mathf.Max(0.1f, settings.cursorSpeed) * Time.deltaTime;
            if (position >= 1f)
            {
                position = 1f;
                direction = -1f;
            }
            else if (position <= 0f)
            {
                position = 0f;
                direction = 1f;
            }
            MoveCursor();

            if (!MinigameInput.ActionPressed) return;

            bool insideZone = position >= targetStart && position <= targetStart + targetWidth;
            if (!insideZone)
            {
                Finish(false);
                return;
            }

            hits++;
            if (hits >= requiredHits)
            {
                Finish(true);
            }
            else
            {
                PickTarget();
            }
        }

        private void PickTarget()
        {
            float margin = GameSettings.Current.targetEdgeMargin;
            float min = margin;
            float max = Mathf.Max(min, 1f - margin - targetWidth);
            targetStart = Random.Range(min, max);

            targetZone.anchorMin = new Vector2(targetStart, 0f);
            targetZone.anchorMax = new Vector2(targetStart + targetWidth, 1f);
            targetZone.offsetMin = Vector2.zero;
            targetZone.offsetMax = Vector2.zero;

            statusText.text = string.Format(GameSettings.Current.hitsFormat, hits, requiredHits);
        }

        private void MoveCursor()
        {
            cursor.anchorMin = new Vector2(position, 0f);
            cursor.anchorMax = new Vector2(position, 1f);
            cursor.sizeDelta = new Vector2(cursorWidth, 0f);
            cursor.anchoredPosition = Vector2.zero;
        }
    }
}
