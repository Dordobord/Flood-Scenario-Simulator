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

    public PreparednessDomain Domain
    {
        get { return domain; }
    }

    public void ShowScore(DomainScore domainScore)
    {
        GameSettings settings = GameSettings.Current;

        nameText.text = settings.GetDomainName(domain);
        pointsText.text = string.Format(settings.domainPointsFormat, domainScore.Points, domainScore.maxPoints);
        fill.fillAmount = domainScore.Points / domainScore.maxPoints;
    }
}
