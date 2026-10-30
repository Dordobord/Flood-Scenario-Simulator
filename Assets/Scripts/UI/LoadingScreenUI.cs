using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    public static LoadingScreenUI Instance { get; private set; }

    [Header("Parts")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
    [FormerlySerializedAs("detailsText")]
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private TMP_Text tipText;
    [SerializeField] private Image progressFill;

    [Header("Animation")]
    [SerializeField] private float fillSpeed = 1f;
    [SerializeField] private float dotSeconds = 0.4f;

    private bool isLoading;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void Load(ScenarioData scenario)
    {
        if (isLoading || scenario == null) return;

        isLoading = true;
        StartCoroutine(LoadScenario(scenario));
    }

    private IEnumerator LoadScenario(ScenarioData scenario)
    {
        panel.SetActive(true);
        progressFill.fillAmount = 0f;
        ShowTitle(scenario);
        ShowRandomTip();

        AsyncOperation loading = SceneManager.LoadSceneAsync(scenario.sceneName);
        loading.allowSceneActivation = false;

        float timer = 0f;
        while (loading.progress < 0.9f || progressFill.fillAmount < 1f)
        {
            timer += Time.unscaledDeltaTime;

            float target = loading.progress / 0.9f;
            UpdateProgressBar(target);
            UpdateLoadingDots(timer);

            yield return null;
        }

        loading.allowSceneActivation = true;
    }

    private void UpdateProgressBar(float target)
    {
        progressFill.fillAmount = Mathf.MoveTowards(progressFill.fillAmount, target, fillSpeed * Time.unscaledDeltaTime);
    }

    private void UpdateLoadingDots(float timer)
    {
        if (loadingText == null) return;

        int dots = Mathf.FloorToInt(timer / dotSeconds) % 4;
        loadingText.text = GameSettings.Current.loadingText + new string('.', dots);
    }

    private void ShowTitle(ScenarioData scenario)
    {
        titleText.text = string.Format(GameSettings.Current.loadingTitleFormat, scenario.displayName);
    }

    private void ShowRandomTip()
    {
        if (tipText == null) return;

        List<string> tips = GameSettings.Current.loadingTips;
        if (tips == null || tips.Count == 0)
        {
            tipText.text = "";
            return;
        }

        string tip = tips[Random.Range(0, tips.Count)];
        tipText.text = string.Format(GameSettings.Current.loadingTipFormat, tip);
    }
}
