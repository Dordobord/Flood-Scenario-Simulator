using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MedicineCard : MonoBehaviour
{
    [Header("Medicine")]
    [SerializeField] private string medicineName = "New Medicine";
    [SerializeField] private string expiry;
    [SerializeField] private bool isNeeded;
    [SerializeField] private Sprite icon;

    [Header("Parts")]
    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text expiryText;
    [SerializeField] private Outline outline;

    private Color backgroundColor;

    public event Action Changed;

    public string MedicineName
    {
        get { return medicineName; }
    }

    public string Expiry
    {
        get { return expiry; }
    }

    public bool IsNeeded
    {
        get { return isNeeded; }
    }

    public bool IsPicked { get; private set; }

    private void Awake()
    {
        backgroundColor = background.color;
        button.onClick.AddListener(Toggle);
    }

    private void OnValidate()
    {
        ShowMedicine();
    }

    public void ShowMedicine()
    {
        if (nameText != null) nameText.text = medicineName;
        if (expiryText != null) expiryText.text = expiry;
        if (iconImage != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = icon != null;
        }
    }

    public void SetPicked(bool picked)
    {
        IsPicked = picked;
        background.color = picked ? GameSettings.Current.medicinePickedColor : backgroundColor;
        if (outline != null) outline.enabled = picked;
    }

    private void Toggle()
    {
        SetPicked(!IsPicked);
        if (Changed != null) Changed();
    }
}
