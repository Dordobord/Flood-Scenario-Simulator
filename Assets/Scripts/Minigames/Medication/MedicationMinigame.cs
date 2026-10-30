using UnityEngine;

public class MedicationMinigame : MinigameBase
{
    [Header("Medication")]
    [SerializeField] private bool shuffleCards = true;

    private MedicineCard[] cards;

    public override bool UsesConfirm
    {
        get { return true; }
    }

    private void Awake()
    {
        cards = GetComponentsInChildren<MedicineCard>(true);
    }

    protected override void Setup()
    {
        foreach (MedicineCard card in cards)
        {
            card.SetPicked(false);
            if (shuffleCards) card.transform.SetSiblingIndex(Random.Range(0, cards.Length));
        }
    }

    public override void Tick()
    {
    }

    public override void Confirm()
    {
        if (IsFinished) return;

        int mistakes = 0;
        foreach (MedicineCard card in cards)
        {
            if (card.IsPicked != card.IsNeeded) mistakes++;
        }

        Mistakes = mistakes;
        Finish(mistakes == 0);
    }
}
