using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace StormWaits
{

    public class GridPackMinigame : MinigameBase
    {
        [SerializeField] private Transform itemsParent;     
        [SerializeField] private RectTransform gridArea;      
        [SerializeField] private RectTransform cellBackground; 
        [SerializeField] private RectTransform placedItemsContainer; 
        [SerializeField] private RectTransform dragLayer; 
        [SerializeField] private GridItem itemPrefab;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private TMP_Text hintText;

        private List<GridItem> tiles = new List<GridItem>();
        private bool[,] occupied;
        private GridItem heldTile;
        private int heldPendingColumn;
        private int heldPendingRow;
        private bool heldPendingValid;
        private int pickupFrame;

        private int packedCount;
        private int totalCorrect;

        protected override void Setup()
        {
            GameSettings gameSettings = GameSettings.Current;

            if (hintText != null) hintText.text = gameSettings.gridHintText;

            if (cellBackground.childCount == 0) BuildGridBackground(gameSettings);

            Vector2 gridPixelSize = new Vector2(gameSettings.gridColumns * gameSettings.gridCellSize, gameSettings.gridRows * gameSettings.gridCellSize);
            gridArea.sizeDelta = gridPixelSize;
            cellBackground.sizeDelta = gridPixelSize;
            placedItemsContainer.sizeDelta = gridPixelSize;
            occupied = new bool[gameSettings.gridColumns, gameSettings.gridRows];

            ClearChildren(itemsParent);
            ClearChildren(placedItemsContainer);
            tiles.Clear();

            heldTile = null;
            packedCount = 0;
            totalCorrect = 0;
            pickupFrame = -1;

            List<SelectChoice> items = new List<SelectChoice>();
            if (settings.choices != null) items.AddRange(settings.choices);
            Shuffle(items);

            foreach (SelectChoice choice in items)
            {
                if (choice.isCorrect) totalCorrect++;

                GridItem tile = Instantiate(itemPrefab, itemsParent);
                tile.Setup(choice);
                tile.SetPoolSize(gameSettings.gridPoolIconSize);
                tile.SetTint(gameSettings.tileColor);
                tile.Clicked += OnTileClicked;
                tiles.Add(tile);
            }

            UpdateStatus();
        }

        public override void Tick()
        {
            if (totalCorrect == 0)
            {
                Finish(true);
                return;
            }

            if (heldTile == null) return;

            GameSettings gameSettings = GameSettings.Current;

            if (MinigameInput.KeyPressedThisFrame(gameSettings.rotateKey))
            {
                heldTile.Rotate();
            }

            UpdateHeldPreview(gameSettings);
            HandlePlacementClick(gameSettings);
        }

        private void UpdateHeldPreview(GameSettings gameSettings)
        {
            Vector2 screenPosition = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
            bool insideGrid = RectTransformUtility.RectangleContainsScreenPoint(gridArea, screenPosition, null);
            heldTile.gameObject.SetActive(true);

            if (!insideGrid)
            {
                heldPendingValid = false;

                if (heldTile.transform.parent != dragLayer)
                {
                    heldTile.transform.SetParent(dragLayer, true);
                }
                heldTile.SetPoolSize(gameSettings.gridPoolIconSize);
                heldTile.SetTint(gameSettings.tileColor);

                heldTile.transform.position = screenPosition;
                return;
            }

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(placedItemsContainer, screenPosition, null, out localPoint);

            int column = Mathf.FloorToInt(localPoint.x / gameSettings.gridCellSize);
            int row = Mathf.FloorToInt(-localPoint.y / gameSettings.gridCellSize);
            column = Mathf.Clamp(column, 0, gameSettings.gridColumns - heldTile.CurrentWidth);
            row = Mathf.Clamp(row, 0, gameSettings.gridRows - heldTile.CurrentHeight);

            bool valid = RegionIsFree(column, row, heldTile.CurrentWidth, heldTile.CurrentHeight, gameSettings) && heldTile.Choice.isCorrect;

            if (heldTile.transform.parent != placedItemsContainer)
            {
                heldTile.transform.SetParent(placedItemsContainer, false);
            }
            heldTile.SetGridSize(gameSettings.gridCellSize);
            heldTile.SetCellPosition(column, row, gameSettings.gridCellSize);
            heldTile.SetTint(valid ? gameSettings.previewValidColor : gameSettings.previewInvalidColor);

            heldPendingColumn = column;
            heldPendingRow = row;
            heldPendingValid = valid;
        }

        private void HandlePlacementClick(GameSettings gameSettings)
        {
            if (Time.frameCount == pickupFrame) return;

            Mouse mouse = Mouse.current;
            if (mouse == null || !mouse.leftButton.wasPressedThisFrame) return;

            Vector2 screenPosition = mouse.position.ReadValue();
            bool insideGrid = RectTransformUtility.RectangleContainsScreenPoint(gridArea, screenPosition, null);

            if (gameSettings.logInput)
            {
                Debug.Log("[Minigame input] GridPack placement click. Inside grid: " + insideGrid + ". Column " + heldPendingColumn + " Row " + heldPendingRow + ". Valid: " + heldPendingValid);
            }

            if (!insideGrid)
            {
                ReturnHeldToPool(gameSettings);
                return;
            }

            if (heldPendingValid)
            {
                PlaceHeldTile(gameSettings);
                return;
            }

            if (!heldTile.Choice.isCorrect)
            {
                
                GridItem tile = heldTile;
                ReturnHeldToPool(gameSettings);
                tile.FlashWrong();

                Mistakes++;
                UpdateStatus();

                int limit = settings.maxMistakes;
                if (limit > 0 && Mistakes >= limit) Finish(false);
                return;
            }

        }

        private void PlaceHeldTile(GameSettings gameSettings)
        {
            SetOccupied(heldPendingColumn, heldPendingRow, heldTile.CurrentWidth, heldTile.CurrentHeight, true);

            heldTile.PlacedColumn = heldPendingColumn;
            heldTile.PlacedRow = heldPendingRow;
            heldTile.IsPlaced = true;
            heldTile.SetTint(gameSettings.packedTileColor);
            heldTile.EndHold();

            heldTile = null;
            packedCount++;
            UpdateStatus();

            if (packedCount >= totalCorrect) Finish(true);
        }

        private void ReturnHeldToPool(GameSettings gameSettings)
        {
            if (heldTile == null) return;

            GridItem tile = heldTile;
            heldTile = null;

            tile.StopFlashing();
            tile.transform.SetParent(itemsParent, false);
            tile.transform.SetAsLastSibling();
            tile.SetPoolSize(gameSettings.gridPoolIconSize);
            tile.SetTint(gameSettings.tileColor);
            tile.EndHold();
        }

        private void OnTileClicked(GridItem tile)
        {
            if (GameSettings.Current.logInput)
            {
                Debug.Log("[Minigame input] GridPack received click on '" + tile.Choice.label + "'. Already holding something: " + (heldTile != null) + ". Attempt finished: " + IsFinished);
            }
            if (heldTile != null || IsFinished) return;

            if (tile.IsPlaced)
            {
                SetOccupied(tile.PlacedColumn, tile.PlacedRow, tile.CurrentWidth, tile.CurrentHeight, false);
                tile.IsPlaced = false;
                packedCount--;
                UpdateStatus();
            }

            heldTile = tile;
            pickupFrame = Time.frameCount;
            tile.BeginHold(GameSettings.Current.dragAlpha);
        }

        private bool RegionIsFree(int column, int row, int width, int height, GameSettings gameSettings)
        {
            if (column < 0 || row < 0) return false;
            if (column + width > gameSettings.gridColumns) return false;
            if (row + height > gameSettings.gridRows) return false;

            for (int c = column; c < column + width; c++)
            {
                for (int r = row; r < row + height; r++)
                {
                    if (occupied[c, r]) return false;
                }
            }
            return true;
        }

        private void SetOccupied(int column, int row, int width, int height, bool value)
        {
            for (int c = column; c < column + width; c++)
            {
                for (int r = row; r < row + height; r++)
                {
                    occupied[c, r] = value;
                }
            }
        }

        private void BuildGridBackground(GameSettings gameSettings)
        {
            GridLayoutGroup layout = cellBackground.gameObject.AddComponent<GridLayoutGroup>();
            layout.cellSize = new Vector2(gameSettings.gridCellSize, gameSettings.gridCellSize);
            layout.spacing = new Vector2(2f, 2f); 
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = gameSettings.gridColumns;

            int cellCount = gameSettings.gridColumns * gameSettings.gridRows;
            for (int i = 0; i < cellCount; i++)
            {
                GameObject cell = new GameObject("Cell", typeof(RectTransform), typeof(Image));
                cell.transform.SetParent(cellBackground, false);

                Image image = cell.GetComponent<Image>();
                image.color = gameSettings.emptyCellColor;
                image.raycastTarget = false;
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
