using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public static bool IsPaused { get; private set; }

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;

    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        panel.SetActive(false);

        if (resumeButton != null) resumeButton.onClick.AddListener(Resume);
        if (retryButton != null) retryButton.onClick.AddListener(Retry);
        if (mainMenuButton != null) mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    private void OnDestroy()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!MinigameInput.KeyPressedThisFrame(GameSettings.Current.pauseKey)) return;

        if (IsPaused)
        {
            Resume();
        }
        else if (CanPause())
        {
            Pause();
        }
    }

    private bool CanPause()
    {
        if (MinigameManager.IsActive || PhaseManager.Instance == null) return false;

        GamePhase phase = PhaseManager.Instance.CurrentPhase;
        return phase == GamePhase.Preparation || phase == GamePhase.Evacuation;
    }

    private void Pause()
    {
        IsPaused = true;
        Time.timeScale = 0f;
        panel.SetActive(true);

        if (titleText != null) titleText.text = GameSettings.Current.pauseTitle;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        panel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void GoToMainMenu()
    {
        string menuScene = GameSettings.Current.mainMenuSceneName;
        if (!Application.CanStreamedLevelBeLoaded(menuScene))
        {
            Debug.LogWarning("PauseMenuUI: the scene \"" + menuScene + "\" is not in Build Settings.");
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(menuScene);
    }
}
