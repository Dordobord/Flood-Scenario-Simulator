using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StormWaits
{
    public class GridItem : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup canvasGroup;

        public event Action<GridItem> Clicked;

        public SelectChoice Choice { get; private set; }

        public int CurrentWidth { get; private set; }
        public int CurrentHeight { get; private set; }

        public int PlacedColumn { get; set; }
        public int PlacedRow { get; set; }
        public bool IsPlaced { get; set; }

        private Coroutine flashRoutine;

        private void Awake()
        {
            button.onClick.AddListener(HandleClick);
        }

        public void Setup(SelectChoice choice)
        {
            Choice = choice;
            CurrentWidth = Mathf.Max(1, choice.gridWidth);
            CurrentHeight = Mathf.Max(1, choice.gridHeight);
            IsPlaced = false;

            UpdateLabel();

            if (choice.icon != null)
            {
                icon.enabled = true;
                icon.sprite = choice.icon;
                icon.color = Color.white;
            }
            else
            {
                icon.enabled = false;
            }

            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            button.interactable = true;
        }

        public void Rotate()
        {
            int temp = CurrentWidth;
            CurrentWidth = CurrentHeight;
            CurrentHeight = temp;
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            label.text = Choice.label + "\n(" + CurrentWidth + "x" + CurrentHeight + ")";
        }

        public void SetPoolSize(float squareSize)
        {
            RectTransform rt = (RectTransform)transform;
            rt.sizeDelta = new Vector2(squareSize, squareSize);
        }

        public void SetGridSize(float cellSize)
        {
            RectTransform rt = (RectTransform)transform;
            rt.sizeDelta = new Vector2(CurrentWidth * cellSize, CurrentHeight * cellSize);
        }

        public void SetCellPosition(int column, int row, float cellSize)
        {
            RectTransform rt = (RectTransform)transform;
            rt.anchoredPosition = new Vector2(column * cellSize, -row * cellSize);
        }

        public void SetTint(Color color)
        {
            background.color = color;
        }

        public void StopFlashing()
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
                flashRoutine = null;
            }
        }

        public void FlashWrong()
        {
            StopFlashing();
            flashRoutine = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            GameSettings settings = GameSettings.Current;
            Color normalColor = background.color;

            background.color = settings.wrongDropColor;
            yield return new WaitForSecondsRealtime(settings.wrongFlashSeconds);
            background.color = normalColor;

            flashRoutine = null;
        }

        public void BeginHold(float alpha)
        {
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = alpha;
            button.interactable = false;
        }

        public void EndHold()
        {
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
            button.interactable = true;
        }

        private void HandleClick()
        {
            if (GameSettings.Current.logInput) Debug.Log("[Minigame input] GridItem clicked: " + Choice.label);
            if (Clicked != null) Clicked(this);
        }
    }
}
