using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MedicationMinigame : MinigameBase
{
    [Header("Medication")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text pouchText;
    [SerializeField] private bool shuffleCards = true;

    private MedicineCard[] cards;

    private void Awake()
    {
        cards = GetComponentsInChildren<MedicineCard>(true);
        foreach (MedicineCard card in cards)
        {
            card.Changed += ShowPouch;
        }
        confirmButton.onClick.AddListener(OnConfirmClicked);
    }

    protected override void Setup()
    {
        foreach (MedicineCard card in cards)
        {
            card.SetPicked(false);
            if (shuffleCards) card.transform.SetSiblingIndex(Random.Range(0, cards.Length));
        }
        ShowPouch();
    }

    public override void Tick()
    {
    }

    private void ShowPouch()
    {
        if (pouchText == null) return;

        GameSettings gameSettings = GameSettings.Current;
        string text = "";

        foreach (MedicineCard card in cards)
        {
            if (!card.IsPicked) continue;

            string line = string.IsNullOrEmpty(card.Expiry) ? card.MedicineName : string.Format(gameSettings.pouchItemFormat, card.MedicineName, card.Expiry);
            text += gameSettings.listBullet + line + "\n";
        }

        pouchText.text = text == "" ? gameSettings.pouchEmptyText : text;
    }

    private void OnConfirmClicked()
    {
        int mistakes = 0;
        foreach (MedicineCard card in cards)
        {
            if (card.IsPicked != card.IsNeeded) mistakes++;
        }

        Mistakes = mistakes;
        Finish(mistakes == 0);
    }
}
