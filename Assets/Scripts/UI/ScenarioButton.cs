using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;

    public ScenarioData Scenario { get; private set; }

    public void Setup(ScenarioData scenario, Action<ScenarioData> onClicked)
    {
        Scenario = scenario;
        label.text = scenario.displayName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClicked(Scenario));

        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        GameSettings settings = GameSettings.Current;
        background.color = selected ? settings.scenarioSelectedColor : settings.scenarioNormalColor;
    }
}
