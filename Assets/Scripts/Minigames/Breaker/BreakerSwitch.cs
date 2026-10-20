using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakerSwitch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private RectTransform lever;
    [SerializeField] private Image leverImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text stateText;

    [Header("Lever")]
    [SerializeField] private Vector2 onPosition;
    [SerializeField] private Vector2 offPosition;

    public bool IsOn { get; private set; }

    public void Setup(string label, Action<BreakerSwitch> clicked)
    {
        nameText.text = label;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => clicked(this));

        SetOn(true);
    }

    public void SetOn(bool on)
    {
        IsOn = on;
        GameSettings settings = GameSettings.Current;

        if (on)
        {
            lever.anchoredPosition = onPosition;
            leverImage.color = settings.breakerOnColor;
            stateText.text = settings.breakerOnText;
        }
        else
        {
            lever.anchoredPosition = offPosition;
            leverImage.color = settings.breakerOffColor;
            stateText.text = settings.breakerOffText;
        }
    }
}
