using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneAppIcon : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text label;

    [Header("App")]
    [SerializeField] private PhonePage page;

    [Header("Done Colors")]
    [SerializeField] private Color doneIconColor = new Color(0.45f, 0.48f, 0.52f);

    private PhoneHomeScreen homeScreen;
    private Color iconColor;
    private string appName;

    public void Init(PhoneHomeScreen owner)
    {
        homeScreen = owner;
        iconColor = iconImage.color;
        appName = label.text;
        button.onClick.AddListener(OnClicked);
    }

    public void Refresh()
    {
        TaskManager tasks = TaskManager.Instance;
        TaskData task = page != null ? page.Task : null;

        bool inScenario = task == null || tasks == null || tasks.ActiveTasks.Contains(task);
        gameObject.SetActive(page != null && inScenario);

        GameSettings settings = GameSettings.Current;
        bool done = task != null && tasks != null && tasks.IsCompleted(task);

        iconImage.color = done ? doneIconColor : iconColor;
        label.text = done ? appName + "\n" + settings.phoneDoneText : appName;
        button.interactable = !done;
    }

    private void OnClicked()
    {
        homeScreen.OpenApp(page);
    }
}
