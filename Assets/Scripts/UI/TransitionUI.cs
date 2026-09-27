using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace StormWaits
{
    public class TransitionUI : MonoBehaviour
    {
        public static TransitionUI Instance { get; private set; }

        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private TMP_Text messageText;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public void Play(Action atBlackout, Action onComplete)
        {
            StartCoroutine(Sequence(atBlackout, onComplete));
        }

        private IEnumerator Sequence(Action atBlackout, Action onComplete)
        {
            GameSettings settings = GameSettings.Current;
            messageText.text = settings.transitionText;
            canvasGroup.blocksRaycasts = true;

            yield return Fade(0f, 1f, settings.transitionFadeSeconds);

            if (atBlackout != null) atBlackout();

            yield return new WaitForSecondsRealtime(settings.transitionHoldSeconds);

            yield return Fade(1f, 0f, settings.transitionFadeSeconds);

            canvasGroup.blocksRaycasts = false;
            if (onComplete != null) onComplete();
        }

        private IEnumerator Fade(float from, float to, float seconds)
        {
            float elapsed = 0f;
            seconds = Mathf.Max(0.01f, seconds);

            while (elapsed < seconds)
            {
                elapsed += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / seconds);
                yield return null;
            }
            canvasGroup.alpha = to;
        }
    }
}
