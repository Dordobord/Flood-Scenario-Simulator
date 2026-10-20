using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PhoneContactRow : MonoBehaviour
{
    [Header("Contact")]
    [SerializeField] private string contactName = "New Contact";
    [SerializeField] private string detail;
    [SerializeField] private bool isNeeded;
    [SerializeField] private Color avatarColor = new Color(0.45f, 0.48f, 0.52f);

    [Header("Parts")]
    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private Image avatar;
    [SerializeField] private TMP_Text initialText;
    [FormerlySerializedAs("label")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text detailText;
    [FormerlySerializedAs("star")]
    [SerializeField] private Image marker;

    private Color backgroundColor;
    private Color markerColor;

    public bool IsNeeded
    {
        get { return isNeeded; }
    }

    public bool IsSaved { get; private set; }

    private void Awake()
    {
        backgroundColor = background.color;
        markerColor = marker.color;
        button.onClick.AddListener(Toggle);
    }

    private void OnValidate()
    {
        ShowContact();
    }

    public void ShowContact()
    {
        if (nameText != null) nameText.text = contactName;
        if (detailText != null) detailText.text = detail;
        if (avatar != null) avatar.color = avatarColor;
        if (initialText != null) initialText.text = string.IsNullOrEmpty(contactName) ? "" : contactName.Substring(0, 1).ToUpper();
    }

    public void SetSaved(bool saved)
    {
        GameSettings settings = GameSettings.Current;

        IsSaved = saved;
        background.color = saved ? settings.contactSavedRowColor : backgroundColor;
        marker.color = saved ? settings.contactMarkerSavedColor : markerColor;
    }

    private void Toggle()
    {
        SetSaved(!IsSaved);
    }
}
