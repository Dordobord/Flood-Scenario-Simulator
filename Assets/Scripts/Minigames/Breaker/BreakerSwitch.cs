using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakerSwitch : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private Image onBlock;
    [SerializeField] private Image offBlock;
    [SerializeField] private Outline slotOutline;
    [SerializeField] private TMP_Text nameText;

    [Header("Colours")]
    [SerializeField] private Color dimColor = new Color(0.16f, 0.16f, 0.16f);

    public bool IsOn { get; private set; }

    private Color onColor;
    private Color offColor;
    private Color outlineColor;
    private bool coloursSaved;

    public void Setup(string label, Action<BreakerSwitch> clicked)
    {
        SaveColours();

        nameText.text = label;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => clicked(this));

        SetWarning(false);
        SetOn(true);
    }

    public void SetOn(bool on)
    {
        IsOn = on;

        if (on)
        {
            onBlock.color = onColor;
            offBlock.color = dimColor;
        }
        else
        {
            onBlock.color = dimColor;
            offBlock.color = offColor;
        }
    }

    public void SetWarning(bool warning)
    {
        if (slotOutline == null) return;

        if (warning) slotOutline.effectColor = GameSettings.Current.failColor;
        else slotOutline.effectColor = outlineColor;
    }

    private void SaveColours()
    {
        if (coloursSaved) return;

        onColor = onBlock.color;
        offColor = offBlock.color;
        if (slotOutline != null) outlineColor = slotOutline.effectColor;
        coloursSaved = true;
    }
}
