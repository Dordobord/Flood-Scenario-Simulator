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

    [Header("Colors")]
    [SerializeField] private Color rowColor = new Color(0.945f, 0.937f, 0.91f);
    [SerializeField] private Color savedRowColor = Color.white;
    [SerializeField] private Color markerColor = new Color(0.706f, 0.698f, 0.663f);
    [SerializeField] private Color savedMarkerColor = new Color(0.729f, 0.459f, 0.09f);

    public bool IsNeeded
    {
        get { return isNeeded; }
    }

    public bool IsSaved { get; private set; }

    private void Awake()
    {
        button.onClick.AddListener(Toggle);
        marker.sprite = GameSettings.Current.contactStarSprite;
        marker.preserveAspect = true;
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
        IsSaved = saved;
        background.color = saved ? savedRowColor : rowColor;
        marker.color = saved ? savedMarkerColor : markerColor;
    }

    public void SetLocked(bool locked)
    {
        button.enabled = !locked;
    }

    private void Toggle()
    {
        SetSaved(!IsSaved);
    }
}
