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
    }

    private void OnGotItClicked()
    {
        Phone.SubmitTask(true, 0);
    }
}
