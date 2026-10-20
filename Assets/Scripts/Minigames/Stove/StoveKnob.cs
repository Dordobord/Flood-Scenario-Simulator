using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoveKnob : MonoBehaviour
{
    [SerializeField] private RectTransform dial;
    [SerializeField] private Image dialImage;
    [SerializeField] private TMP_Text nameText;

    public bool IsOff { get; private set; }
    public float Angle { get; private set; }

    public RectTransform Dial
    {
        get { return dial; }
    }

    public void Setup(string label, float startAngle)
    {
        nameText.text = label;
        IsOff = false;
        dialImage.color = GameSettings.Current.stoveOnColor;
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
        dialImage.color = GameSettings.Current.stoveOffColor;
        SetAngle(offAngle);
    }
}
