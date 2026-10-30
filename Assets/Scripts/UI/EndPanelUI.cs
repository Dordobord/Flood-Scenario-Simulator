using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Results")]
    [SerializeField] private TMP_Text totalText;
    [SerializeField] private DomainBarUI[] domainBars;
    [SerializeField] private TMP_Text recommendationsText;

    private void Awake()
    {
        panel.SetActive(false);

        retryButton.onClick.AddListener(Retry);
        mainMenuButton.onClick.AddListener(GoToMainMenu);

        GameEvents.EvacuationCompleted += Show;
    }

    private void OnDestroy()
    {
        GameEvents.EvacuationCompleted -= Show;
    }

    private void Show()
    {
        panel.SetActive(true);
        ShowResults();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ShowResults()
    {
        TaskManager taskManager = TaskManager.Instance;
        PreparednessScore score = ScoreCalculator.Calculate(taskManager.ActiveTasks, taskManager, PhaseManager.Instance.EvacuationSeconds);
        GameSettings settings = GameSettings.Current;

        totalText.text = string.Format(settings.totalPointsFormat, score.totalPoints, score.maxPoints);
        ShowDomainBars(score);
        recommendationsText.text = BuildRecommendations(score, settings);
    }

    private void ShowDomainBars(PreparednessScore score)
    {
        foreach (DomainBarUI bar in domainBars)
        {
            DomainScore domainScore = FindDomainScore(score, bar.Domain);

            if (domainScore == null)
            {
                bar.gameObject.SetActive(false);
                continue;
            }

            bar.ShowScore(domainScore);
        }
    }

    private DomainScore FindDomainScore(PreparednessScore score, PreparednessDomain domain)
    {
        foreach (DomainScore domainScore in score.domainScores)
        {
            if (domainScore.domain == domain) return domainScore;
        }
        return null;
    }

    private string BuildRecommendations(PreparednessScore score, GameSettings settings)
    {
        if (score.weakestDomains.Count == 0) return settings.allDoneMessage;

        string text = "";

        if (score.strongestDomains.Count > 0)
        {
            text += string.Format(settings.praiseFormat, JoinDomainNames(score.strongestDomains, settings)) + "\n\n";
        }

        foreach (PreparednessDomain domain in score.weakestDomains)
        {
            text += settings.GetDomainHint(domain) + "\n\n";
        }
        return text.TrimEnd();
    }

    private string JoinDomainNames(List<PreparednessDomain> domains, GameSettings settings)
    {
        string names = "";
        for (int i = 0; i < domains.Count; i++)
        {
            if (i > 0 && i == domains.Count - 1) names += settings.lastDomainSeparator;
            else if (i > 0) names += settings.domainSeparator;

            names += settings.GetDomainName(domains[i]);
        }
        return names;
    }

    private void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        SceneManager.LoadScene(GameSettings.Current.mainMenuSceneName);
    }
}
