using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScenarioButton : MonoBehaviour
{
    [Header("Scenario")]
    [SerializeField] private ScenarioData scenario;

    [Header("Parts")]
    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;

    public ScenarioData Scenario
    {
        get { return scenario; }
    }

    private void OnValidate()
    {
        if (scenario != null && label != null) label.text = scenario.displayName;
    }

    public void Init(Action<ScenarioData> onClicked)
    {
        if (scenario != null) label.text = scenario.displayName;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClicked(scenario));

        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        GameSettings settings = GameSettings.Current;
        background.color = selected ? settings.scenarioSelectedColor : settings.scenarioNormalColor;
    }
}
