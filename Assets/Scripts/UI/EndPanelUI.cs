using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class EndPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    [Header("Results")]
    [SerializeField] private TMP_Text totalText;
    [SerializeField] private DomainBarUI[] domainBars;
    [FormerlySerializedAs("recommendationsText")]
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private Image[] totalStars;
    [SerializeField] private TMP_Text closingText;
    [SerializeField] private DomainResultPopup domainPopup;

    [Header("Slides")]
    [SerializeField] private ResultSlider slider;
    [SerializeField] private TaskCardUI[] taskCards;

    [Header("Star Colors")]
    [SerializeField] private Color starOnColor = new Color(1f, 0.8f, 0.2f);
    [SerializeField] private Color starOffColor = new Color(0.122f, 0.141f, 0.169f);

    private PreparednessScore score;

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
        slider.ShowFirst();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void ShowResults()
    {
        TaskManager taskManager = TaskManager.Instance;
        score = ScoreCalculator.Calculate(taskManager.ActiveTasks, taskManager, PhaseManager.Instance.EvacuationSeconds);
        GameSettings settings = GameSettings.Current;

        totalText.text = string.Format(settings.totalPointsFormat, score.totalPoints, score.maxPoints);
        ShowDomainBars();
        ShowTotalStars(settings);
        ShowTaskCards(taskManager);
        promptText.text = settings.resultsPrompt;
    }

    private void ShowDomainBars()
    {
        foreach (DomainBarUI bar in domainBars)
        {
            DomainScore domainScore = FindDomainScore(bar.Domain);

            if (domainScore == null)
            {
                bar.gameObject.SetActive(false);
                continue;
            }

            bar.ShowScore(domainScore, ShowDomainPopup);
        }
    }

    private void ShowTotalStars(GameSettings settings)
    {
        int earned = 0;
        foreach (DomainScore domainScore in score.domainScores)
        {
            if (domainScore.HasStar) earned++;
        }

        for (int i = 0; i < totalStars.Length; i++)
        {
            totalStars[i].gameObject.SetActive(i < score.domainScores.Count);
            totalStars[i].color = i < earned ? starOnColor : starOffColor;
        }

        string line = earned == score.domainScores.Count ? settings.resultsPerfectLine : settings.resultsPartialLine;
        closingText.text = line + "\n" + string.Format(settings.starMeaningFormat, earned, score.domainScores.Count);
    }

    private void ShowTaskCards(TaskManager taskManager)
    {
        foreach (TaskCardUI card in taskCards)
        {
            if (FindDomainScore(card.Domain) == null)
            {
                card.gameObject.SetActive(false);
                continue;
            }

            card.Show(taskManager.ActiveTasks);
        }
    }

    private void ShowDomainPopup(DomainScore domainScore)
    {
        domainPopup.Show(domainScore);
    }

    private DomainScore FindDomainScore(PreparednessDomain domain)
    {
        foreach (DomainScore domainScore in score.domainScores)
        {
            if (domainScore.domain == domain) return domainScore;
        }
        return null;
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
