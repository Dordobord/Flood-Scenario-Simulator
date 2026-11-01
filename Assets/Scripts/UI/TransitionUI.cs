using System;
using System.Collections;
using TMPro;
using UnityEngine;

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

    public void PlayArrival(Action onComplete)
    {
        StartCoroutine(ArrivalSequence(onComplete));
    }

    private IEnumerator Sequence(Action atBlackout, Action onComplete)
    {
        GameSettings settings = GameSettings.Current;
        messageText.text = settings.transitionText;
        messageText.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        yield return Fade(0f, 1f, settings.transitionFadeSeconds, SetScreenAlpha);

        if (atBlackout != null) atBlackout();

        float holdSeconds = settings.transitionHoldSeconds;
        AudioClip gateCreak = SoundLibrary.Current.gateCreak;
        if (gateCreak != null) holdSeconds = Mathf.Max(holdSeconds, gateCreak.length - settings.transitionFadeSeconds);

        yield return new WaitForSecondsRealtime(holdSeconds);

        yield return Fade(1f, 0f, settings.transitionFadeSeconds, SetScreenAlpha);

        canvasGroup.blocksRaycasts = false;
        if (onComplete != null) onComplete();
    }

    private IEnumerator ArrivalSequence(Action onComplete)
    {
        GameSettings settings = GameSettings.Current;
        messageText.text = settings.arrivalText;
        messageText.alpha = 0f;
        canvasGroup.blocksRaycasts = true;

        yield return Fade(0f, 1f, settings.arrivalFadeSeconds, SetScreenAlpha);
        yield return Fade(0f, 1f, settings.arrivalFadeSeconds, SetTextAlpha);
        yield return new WaitForSecondsRealtime(settings.arrivalHoldSeconds);
        yield return Fade(1f, 0f, settings.arrivalFadeSeconds, SetTextAlpha);

        canvasGroup.blocksRaycasts = false;
        if (onComplete != null) onComplete();
    }

    private void SetScreenAlpha(float alpha)
    {
        canvasGroup.alpha = alpha;
    }

    private void SetTextAlpha(float alpha)
    {
        messageText.alpha = alpha;
    }

    private IEnumerator Fade(float from, float to, float seconds, Action<float> apply)
    {
        float elapsed = 0f;
        seconds = Mathf.Max(0.01f, seconds);

        while (elapsed < seconds)
        {
            elapsed += Time.unscaledDeltaTime;
            apply(Mathf.Lerp(from, to, elapsed / seconds));
            yield return null;
        }
        apply(to);
    }
}
