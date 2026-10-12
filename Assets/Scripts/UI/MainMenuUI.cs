using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameSettings settings;
    [SerializeField] private List<ScenarioData> scenarios = new List<ScenarioData>();

    [Header("Scenario List")]
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private ScenarioButton buttonPrefab;

    [Header("Text")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text preparationText;
    [SerializeField] private TMP_Text descriptionText;

    [Header("Buttons")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private List<ScenarioButton> buttons = new List<ScenarioButton>();
    private ScenarioData selected;

    private void Awake()
    {
        if (settings != null) GameSettings.Current = settings;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameSettings current = GameSettings.Current;
        if (titleText != null) titleText.text = current.menuTitle;
        if (subtitleText != null) subtitleText.text = current.menuSubtitle;

        if (startButton != null) startButton.onClick.AddListener(StartScenario);
        if (quitButton != null) quitButton.onClick.AddListener(Quit);

        BuildButtons();
        ShowDetails(null);
    }

    private void BuildButtons()
    {
        if (buttonContainer == null || buttonPrefab == null)
        {
            Debug.LogWarning("MainMenuUI: Button Container or Button Prefab is not assigned.");
            return;
        }

        foreach (ScenarioData scenario in scenarios)
        {
            if (scenario == null) continue;

            ScenarioButton button = Instantiate(buttonPrefab, buttonContainer);
            button.Setup(scenario, Select);
            buttons.Add(button);
        }
    }

    private void Select(ScenarioData scenario)
    {
        selected = scenario;

        foreach (ScenarioButton button in buttons)
        {
            button.SetSelected(button.Scenario == selected);
        }

        ShowDetails(selected);
    }

    private void ShowDetails(ScenarioData scenario)
    {
        GameSettings current = GameSettings.Current;

        if (scenario == null)
        {
            if (nameText != null) nameText.text = current.noScenarioSelectedTitle;
            if (preparationText != null) preparationText.text = "";
            if (descriptionText != null) descriptionText.text = current.noScenarioSelectedMessage;
            if (startButton != null) startButton.interactable = false;
            return;
        }

        float seconds = scenario.preparationSeconds > 0f ? scenario.preparationSeconds : current.preparationSeconds;

        if (nameText != null) nameText.text = scenario.displayName;
        if (preparationText != null) preparationText.text = string.Format(current.preparationTimeFormat, FormatTime(seconds));
        if (descriptionText != null) descriptionText.text = scenario.description;
        if (startButton != null) startButton.interactable = true;
    }

    private void StartScenario()
    {
        if (selected == null) return;

        if (string.IsNullOrEmpty(selected.sceneName) || !Application.CanStreamedLevelBeLoaded(selected.sceneName))
        {
            Debug.LogWarning("MainMenuUI: the scene \"" + selected.sceneName + "\" for " + selected.displayName + " is not in Build Settings.");
            return;
        }

        SceneManager.LoadScene(selected.sceneName);
    }

    private void Quit()
    {
        Application.Quit();
    }

    private string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
        return minutes.ToString("00") + ":" + remainingSeconds.ToString("00");
    }
}
