using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StormWaits
{
    public class ChoiceButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image background;
        [SerializeField] private TMP_Text label;

        public SelectChoice Choice { get; private set; }
        public bool IsSelected { get; private set; }

        public void Setup(SelectChoice choice)
        {
            Choice = choice;
            IsSelected = false;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(Toggle);

            Refresh();
        }

        private void Toggle()
        {
            IsSelected = !IsSelected;
            Refresh();
        }

        private void Refresh()
        {
            GameSettings settings = GameSettings.Current;

            if (IsSelected)
            {
                background.color = settings.choiceSelectedColor;
                label.text = settings.selectedMark + Choice.label;
            }
            else
            {
                background.color = settings.choiceNormalColor;
                label.text = settings.unselectedMark + Choice.label;
            }
        }
    }
}
