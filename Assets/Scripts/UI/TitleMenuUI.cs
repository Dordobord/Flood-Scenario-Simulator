using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMenuUI : MonoBehaviour
{
    [SerializeField] private GameSettings settings;

    [Header("Buttons")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Awake()
    {
        if (settings != null) GameSettings.Current = settings;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (playButton != null) playButton.onClick.AddListener(OpenScenarioSelector);
        if (quitButton != null) quitButton.onClick.AddListener(Quit);
    }

    private void OpenScenarioSelector()
    {
        SceneManager.LoadScene(GameSettings.Current.scenarioSelectorSceneName);
    }

    private void Quit()
    {
        Application.Quit();
    }
}
