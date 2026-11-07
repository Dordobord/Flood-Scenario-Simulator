using System;
using UnityEngine;

public class PhoneHomeScreen : MonoBehaviour
{
    public event Action<PhonePage> AppChosen;

    [Header("Alert")]
    [SerializeField] private PhoneAlertCard alertCard;

    private PhoneAppIcon[] icons;

    private void Update()
    {
        if (alertCard != null) alertCard.Refresh();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        if (icons == null) FindIcons();

        foreach (PhoneAppIcon icon in icons)
        {
            icon.Refresh();
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OpenApp(PhonePage page)
    {
        PhoneAlertCard.IsPending = false;
        if (AppChosen != null) AppChosen(page);
    }

    private void FindIcons()
    {
        icons = GetComponentsInChildren<PhoneAppIcon>(true);

        foreach (PhoneAppIcon icon in icons)
        {
            icon.Init(this);
        }
    }
}
