using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneContactsPage : PhonePage
{
    [Header("Contacts")]
    [SerializeField] private Button saveButton;
    [SerializeField] private bool shuffleRows = true;

    [Header("Help Card")]
    [SerializeField] private GameObject helpCard;
    [SerializeField] private TMP_Text helpTitleText;
    [SerializeField] private TMP_Text helpMessageText;
    [SerializeField] private Button helpButton;

    private PhoneContactRow[] rows;
    private bool hasSeenHelp;

    private void Awake()
    {
        GameSettings settings = GameSettings.Current;

        rows = GetComponentsInChildren<PhoneContactRow>(true);
        saveButton.onClick.AddListener(OnSaveClicked);

        TMP_Text buttonText = saveButton.GetComponentInChildren<TMP_Text>(true);
        buttonText.text = settings.contactConfirmText;

        if (helpCard == null) return;

        helpTitleText.text = settings.contactHelpTitle;
        helpMessageText.text = settings.contactHelpMessage;
        helpButton.GetComponentInChildren<TMP_Text>(true).text = settings.contactHelpButtonText;
        helpButton.onClick.AddListener(CloseHelp);
    }

    public override void Open()
    {
        base.Open();

        saveButton.interactable = true;
        if (helpCard != null) helpCard.SetActive(!hasSeenHelp);

        foreach (PhoneContactRow row in rows)
        {
            row.SetSaved(false);
            row.SetLocked(false);
            if (shuffleRows) row.transform.SetSiblingIndex(Random.Range(0, rows.Length));
        }
    }

    private void CloseHelp()
    {
        hasSeenHelp = true;
        helpCard.SetActive(false);
    }

    private void OnSaveClicked()
    {
        int mistakes = 0;
        foreach (PhoneContactRow row in rows)
        {
            if (row.IsSaved != row.IsNeeded) mistakes++;
        }

        if (mistakes > 0)
        {
            Phone.SubmitTask(false, mistakes);
            return;
        }

        StartCoroutine(ShowSortedContacts());
    }

    private IEnumerator ShowSortedContacts()
    {
        saveButton.interactable = false;
        MoveStarredToTop();

        yield return new WaitForSeconds(GameSettings.Current.contactSortSeconds);

        Phone.SubmitTask(true, 0);
    }

    private void MoveStarredToTop()
    {
        int top = 0;

        foreach (PhoneContactRow row in rows)
        {
            row.SetLocked(true);
            if (!row.IsSaved) continue;

            row.transform.SetSiblingIndex(top);
            top++;
        }
    }
}
