using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomainResultPopup : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text mustDoText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    public void Show(DomainScore domainScore)
    {
        GameSettings settings = GameSettings.Current;
        DomainDisplayName display = settings.GetDomainDisplay(domainScore.domain);
        if (display == null) return;

        iconImage.sprite = display.icon;
        iconImage.enabled = display.icon != null;
        titleText.text = settings.GetDomainName(domainScore.domain);
        messageText.text = GetMessage(domainScore, display) + "\n" + display.hint;
        mustDoText.text = string.Format(settings.mustDoFormat, JoinTaskNames(display.mustDoTasks, settings));

        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private string GetMessage(DomainScore domainScore, DomainDisplayName display)
    {
        if (domainScore.IsPerfect) return display.greatMessage;
        if (domainScore.HasStar) return display.goodMessage;
        return display.weakMessage;
    }

    private string JoinTaskNames(List<TaskData> tasks, GameSettings settings)
    {
        string names = "";
        for (int i = 0; i < tasks.Count; i++)
        {
            if (i > 0) names += settings.taskSeparator;
            names += tasks[i].ResultName;
        }
        return names;
    }
}
