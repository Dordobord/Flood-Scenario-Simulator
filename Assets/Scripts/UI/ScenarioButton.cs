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

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.6f, 0.68f, 0.76f);

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
        background.color = selected ? selectedColor : normalColor;
    }
}
