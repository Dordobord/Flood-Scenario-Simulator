using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneAlertCard : MonoBehaviour
{
    public static bool IsPending { get; set; }

    [Header("Parts")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        GameSettings settings = GameSettings.Current;

        titleText.text = settings.phoneAlertTitle;
        timeText.text = settings.phoneAlertTime;
        messageText.text = settings.hazardAlertMessage;
    }

    public void Refresh()
    {
        if (gameObject.activeSelf != IsPending) gameObject.SetActive(IsPending);
    }

    private void Close()
    {
        IsPending = false;
        gameObject.SetActive(false);
    }
}
