using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneMinigame : MinigameBase
{
    [Header("Screen")]
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private Button homeButton;

    [Header("Pages")]
    [SerializeField] private PhoneHomeScreen homeScreen;

    public event Action HomePressed;

    private PhonePage[] pages;

    public PhoneHomeScreen HomeScreen
    {
        get { return homeScreen; }
    }

    private void Awake()
    {
        homeButton.onClick.AddListener(OnHomeClicked);

        pages = GetComponentsInChildren<PhonePage>(true);
        foreach (PhonePage page in pages)
        {
            page.Init(this);
        }
    }

    private void Update()
    {
        timeText.text = DateTime.Now.ToString(GameSettings.Current.phoneTimeFormat);
    }

    public void ShowHome()
    {
        gameObject.SetActive(true);
        CloseAllPages();
        homeScreen.Show();
    }

    public void ShowPage(PhonePage page)
    {
        gameObject.SetActive(true);
        homeScreen.Hide();
        CloseAllPages();
        page.Open();
    }

    protected override void Setup()
    {
        PhonePage page = FindTaskPage();
        if (page == null)
        {
            Debug.LogWarning("PhoneMinigame: no page has this task assigned.");
            ShowHome();
            return;
        }

        ShowPage(page);
    }

    public override void Tick()
    {
    }

    public void SubmitTask(bool success, int mistakes)
    {
        if (IsFinished) return;

        Mistakes = mistakes;
        Finish(success);
    }

    private PhonePage FindTaskPage()
    {
        foreach (PhonePage page in pages)
        {
            if (page.Task != null && page.Task.minigame == settings) return page;
        }
        return null;
    }

    private void CloseAllPages()
    {
        foreach (PhonePage page in pages)
        {
            page.Close();
        }
    }

    private void OnHomeClicked()
    {
        if (HomePressed != null) HomePressed();
    }
}
