using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StormWaits
{
    public class DragToBagMinigame : MinigameBase
    {
        [SerializeField] private Transform itemsParent;
        [SerializeField] private RectTransform bagArea;
        [SerializeField] private Transform bagContent;
        [SerializeField] private RectTransform dragLayer;
        [SerializeField] private ItemTile itemTilePrefab;
        [SerializeField] private TMP_Text statusText;

        private List<ItemTile> tiles = new List<ItemTile>();
        private int packedCount;
        private int totalCorrect;

        protected override void Setup()
        {
            
            ClearChildren(itemsParent);
            ClearChildren(bagContent);
            ClearChildren(dragLayer);
            tiles.Clear();

            packedCount = 0;
            totalCorrect = 0;

            List<SelectChoice> items = new List<SelectChoice>();
            if (settings.choices != null) items.AddRange(settings.choices);
            Shuffle(items);

            foreach (SelectChoice item in items)
            {
                if (item.isCorrect) totalCorrect++;

                ItemTile tile = Instantiate(itemTilePrefab, itemsParent);
                tile.Setup(item, dragLayer);
                tile.Dropped += OnTileDropped;
                tiles.Add(tile);
            }

            if (totalCorrect == 0)
            {
                Debug.LogWarning("Drag To Bag: this task has no correct items. Tick 'Is Correct' on at least one choice.");
            }

            UpdateStatus();
        }

        public override void Tick()
        {
            if (totalCorrect == 0) Finish(true);
        }

        private void OnTileDropped(ItemTile tile, Vector2 screenPosition)
        {
            if (IsFinished)
            {
                tile.ReturnHome();
                return;
            }

            bool overBag = RectTransformUtility.RectangleContainsScreenPoint(bagArea, screenPosition, null);
            if (!overBag)
            {
                tile.ReturnHome();
                return;
            }

            if (tile.Choice.isCorrect)
            {
                tile.PackInto(bagContent);
                packedCount++;
                UpdateStatus();

                if (packedCount >= totalCorrect) Finish(true);
            }
            else
            {
                Mistakes++;
                tile.ReturnHome();
                tile.FlashWrong();
                UpdateStatus();

                int limit = settings.maxMistakes;
                if (limit > 0 && Mistakes >= limit) Finish(false);
            }
        }

        private void UpdateStatus()
        {
            GameSettings gameSettings = GameSettings.Current;

            if (settings.maxMistakes > 0)
            {
                statusText.text = string.Format(gameSettings.packedLimitFormat, packedCount, totalCorrect, Mistakes, settings.maxMistakes);
            }
            else
            {
                statusText.text = string.Format(gameSettings.packedFormat, packedCount, totalCorrect, Mistakes);
            }
        }
    }
}
