using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoveKnob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform dial;
    [SerializeField] private Image dialImage;
    [SerializeField] private TMP_Text statusText;

    [Header("Colours")]
    [SerializeField] private Color onColor = new Color(0.75f, 0.22f, 0.17f);
    [SerializeField] private Color offColor = new Color(0.33f, 0.33f, 0.33f);

    public bool IsOff { get; private set; }
    public float Angle { get; private set; }

    public RectTransform Dial
    {
        get { return dial; }
    }

    public void Setup(float startAngle)
    {
        IsOff = false;
        dialImage.color = onColor;
        if (statusText != null) statusText.text = GameSettings.Current.stoveOnText;
        SetAngle(startAngle);
    }

    public void SetAngle(float angle)
    {
        Angle = Mathf.Repeat(angle, 360f);
        dial.localEulerAngles = new Vector3(0f, 0f, Angle);
    }

    public void TurnOff(float offAngle)
    {
        IsOff = true;
        dialImage.color = offColor;
        if (statusText != null) statusText.text = GameSettings.Current.stoveOffText;
        SetAngle(offAngle);
    }
}
