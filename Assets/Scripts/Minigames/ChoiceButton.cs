using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField] protected Button button;
    [SerializeField] protected Image background;
    [SerializeField] protected TMP_Text label;

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

    protected virtual void Refresh()
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
