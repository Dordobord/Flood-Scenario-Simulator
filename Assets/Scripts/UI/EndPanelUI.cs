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
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text tasksText;
    [SerializeField] private TMP_Text recommendationsText;

    private void Awake()
    {
        panel.SetActive(false);

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(Retry);
        }
        else
        {
            Debug.LogWarning("EndPanelUI: Retry Button is not assigned.");
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }
        else
        {
            Debug.LogWarning("EndPanelUI: Main Menu Button is not assigned.");
        }

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
        if (TaskManager.Instance == null || PhaseManager.Instance == null)
        {
            Debug.LogWarning("EndPanelUI: the results need a TaskManager and a PhaseManager in the scene.");
            return;
        }

        PreparednessScore score = ScoreCalculator.Calculate(
            TaskManager.Instance.ActiveTasks, TaskManager.Instance, PhaseManager.Instance.EvacuationSeconds);

        GameSettings settings = GameSettings.Current;
        if (settings.showScore)
        {
            if (scoreText != null) scoreText.text = BuildScoreText(score, settings);
            if (tasksText != null) tasksText.text = BuildTasksText(score, settings);
            if (recommendationsText != null) recommendationsText.text = BuildRecommendationsText(score, settings);
        }
        else
        {
            if (scoreText != null) scoreText.text = BuildSummaryText(score, settings);
            if (tasksText != null) tasksText.text = BuildTaskNamesText(score, settings);
            if (recommendationsText != null) recommendationsText.text = BuildMissedText(score, settings);
        }

        if (settings.logInput)
        {
            Debug.Log("Results: " + score.completedCount + "/" + score.totalCount + " tasks, " + score.percent + "%, " + score.rating + ", evacuation " + score.evacuationSeconds + " s");
        }
    }

    private string BuildScoreText(PreparednessScore score, GameSettings settings)
    {
        string text = string.Format(settings.scorePercentFormat, score.percent) + "\n";
        text += string.Format(settings.ratingFormat, score.rating) + "\n";
        if (score.ratingCapped) text += settings.ratingCappedNote + "\n";
        text += string.Format(settings.evacuationTimeFormat, FormatTime(score.evacuationSeconds)) + "\n";
        text += string.Format(settings.potholeCountFormat, PhaseManager.Instance.PotholesSteppedIn) + "\n\n";

        foreach (DomainScore domainScore in score.domainScores)
        {
            string domainName = settings.GetDomainName(domainScore.domain);
            text += string.Format(settings.domainPercentFormat, domainName, domainScore.Percent) + "\n";
        }
        return text;
    }

    private string BuildSummaryText(PreparednessScore score, GameSettings settings)
    {
        string text = string.Format(settings.tasksDoneFormat, score.completedCount, score.totalCount) + "\n";
        text += string.Format(settings.evacuationTimeFormat, FormatTime(score.evacuationSeconds)) + "\n";
        text += string.Format(settings.potholeCountFormat, PhaseManager.Instance.PotholesSteppedIn);
        return text;
    }

    private string BuildTaskNamesText(PreparednessScore score, GameSettings settings)
    {
        if (score.completedTasks.Count == 0) return settings.noTasksCompletedMessage;

        string text = "";
        foreach (TaskData task in score.completedTasks)
        {
            text += settings.listBullet + task.title + "\n";
        }
        return text;
    }

    private string BuildMissedText(PreparednessScore score, GameSettings settings)
    {
        if (score.missedTasks.Count == 0) return settings.allDoneMessage;

        string text = settings.missedTasksHeading + "\n";
        foreach (TaskData task in score.missedTasks)
        {
            text += settings.listBullet + GetRecommendation(task, settings) + "\n";
        }
        return text;
    }

    private string BuildTasksText(PreparednessScore score, GameSettings settings)
    {
        if (score.completedTasks.Count == 0) return settings.noTasksCompletedMessage;

        string text = "";
        foreach (TaskData task in score.completedTasks)
        {
            text += settings.listBullet + string.Format(settings.taskScoreFormat, task.title, score.taskPercents[task]) + "\n";
        }
        return text;
    }

    private string BuildRecommendationsText(PreparednessScore score, GameSettings settings)
    {
        if (score.IsPerfect) return settings.allDoneMessage;

        string text = "";

        if (score.missedCriticalTasks.Count > 0)
        {
            text += settings.missedCriticalHeading + "\n";
            foreach (TaskData task in score.missedCriticalTasks)
            {
                text += settings.listBullet + GetRecommendation(task, settings) + "\n";
            }
            text += "\n";
        }

        if (score.weakestDomains.Count == 0) return text;

        List<string> domainNames = new List<string>();
        foreach (PreparednessDomain domain in score.weakestDomains)
        {
            domainNames.Add(settings.GetDomainName(domain));
        }

        text += string.Format(settings.weakestDomainFormat, string.Join(settings.domainSeparator, domainNames)) + "\n";

        foreach (TaskData task in score.missedTasks)
        {
            if (!score.weakestDomains.Contains(task.domain) || score.missedCriticalTasks.Contains(task)) continue;
            text += settings.listBullet + GetRecommendation(task, settings) + "\n";
        }
        return text;
    }

    private string GetRecommendation(TaskData task, GameSettings settings)
    {
        if (string.IsNullOrEmpty(task.recommendation)) return task.title;
        return string.Format(settings.recommendationFormat, task.title, task.recommendation);
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
        return minutes.ToString("00") + ":" + remainingSeconds.ToString("00");
    }

    private void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        string menuScene = GameSettings.Current.mainMenuSceneName;
        if (!Application.CanStreamedLevelBeLoaded(menuScene))
        {
            Debug.LogWarning("EndPanelUI: the scene \"" + menuScene + "\" is not in Build Settings.");
            return;
        }
        SceneManager.LoadScene(menuScene);
    }
}
