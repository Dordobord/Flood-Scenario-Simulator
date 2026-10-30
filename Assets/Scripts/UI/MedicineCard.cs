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

    [Header("Colours")]
    [SerializeField] private Color pickedColor = new Color(0.184f, 0.29f, 0.212f);
    [SerializeField] private Color pickedOutlineColor = new Color(0.247f, 0.682f, 0.353f);

    private Color backgroundColor;
    private Color outlineColor;

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
        if (outline != null) outlineColor = outline.effectColor;
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
        if (iconImage != null && icon != null) iconImage.sprite = icon;
    }

    public void SetPicked(bool picked)
    {
        IsPicked = picked;
        background.color = picked ? pickedColor : backgroundColor;
        if (outline != null) outline.effectColor = picked ? pickedOutlineColor : outlineColor;
    }

    private void Toggle()
    {
        SetPicked(!IsPicked);
        if (Changed != null) Changed();
    }
}
