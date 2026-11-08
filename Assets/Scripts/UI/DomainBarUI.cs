using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DomainBarUI : MonoBehaviour
{
    [Header("Domain")]
    [SerializeField] private PreparednessDomain domain;

    [Header("Parts")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image fill;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private Button button;

    private DomainScore shownScore;
    private Action<DomainScore> onClicked;

    public PreparednessDomain Domain
    {
        get { return domain; }
    }

    private void Awake()
    {
        button.onClick.AddListener(Click);
    }

    public void ShowScore(DomainScore domainScore, Action<DomainScore> clicked)
    {
        GameSettings settings = GameSettings.Current;

        shownScore = domainScore;
        onClicked = clicked;

        nameText.text = settings.GetDomainName(domain);
        pointsText.text = string.Format(settings.domainPointsFormat, domainScore.Points, domainScore.maxPoints);
        fill.fillAmount = domainScore.Points / domainScore.maxPoints;
    }

    private void Click()
    {
        if (onClicked != null) onClicked(shownScore);
    }
}
