using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneHazardMapPage : PhonePage
{
    [Header("Hazard Map")]
    [SerializeField] private TMP_Text warningText;
    [SerializeField] private Button gotItButton;

    private void Awake()
    {
        gotItButton.onClick.AddListener(OnGotItClicked);
    }

    public override void Open()
    {
        base.Open();
        warningText.text = GameSettings.Current.hazardWarningText;
        ShowScenarioMap();
    }

    private void ShowScenarioMap()
    {
        ScenarioData scenario = TaskManager.Instance != null ? TaskManager.Instance.CurrentScenario : null;
        string wanted = scenario != null ? "Map_" + scenario.type : null;

        Transform match = null;
        foreach (Transform child in transform)
        {
            if (child.name == wanted || (match == null && child.name == "Map")) match = child;
        }

        foreach (Transform child in transform)
        {
            if (child.name == "Map" || child.name.StartsWith("Map_")) child.gameObject.SetActive(child == match);
        }
    }

    private void OnGotItClicked()
    {
        Phone.SubmitTask(true, 0);
    }
}
