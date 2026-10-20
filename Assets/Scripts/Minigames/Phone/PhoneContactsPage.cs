using UnityEngine;
using UnityEngine.UI;

public class PhoneContactsPage : PhonePage
{
    [Header("Contacts")]
    [SerializeField] private Button saveButton;
    [SerializeField] private bool shuffleRows = true;

    private PhoneContactRow[] rows;

    private void Awake()
    {
        rows = GetComponentsInChildren<PhoneContactRow>(true);
        saveButton.onClick.AddListener(OnSaveClicked);
    }

    public override void Open()
    {
        base.Open();

        foreach (PhoneContactRow row in rows)
        {
            row.SetSaved(false);
            if (shuffleRows) row.transform.SetSiblingIndex(Random.Range(0, rows.Length));
        }
    }

    private void OnSaveClicked()
    {
        int mistakes = 0;
        foreach (PhoneContactRow row in rows)
        {
            if (row.IsSaved != row.IsNeeded) mistakes++;
        }

        Phone.SubmitTask(mistakes == 0, mistakes);
    }
}
