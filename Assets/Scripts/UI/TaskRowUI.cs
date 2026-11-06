using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskRowUI : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;

    [Header("Colors")]
    [SerializeField] private Color mustDoRowColor = new Color(0.941f, 0.659f, 0.271f);
    [SerializeField] private Color mustDoTextColor = new Color(0.118f, 0.133f, 0.157f);
    [SerializeField] private Color finishedTextColor = Color.white;
    [SerializeField] private Color missedTextColor = new Color(1f, 0.612f, 0.541f);

    public void Show(TaskData task, TaskRowState state)
    {
        label.text = task.ResultName;
        label.color = GetTextColor(state);
        background.enabled = state == TaskRowState.MustDo;
        background.color = mustDoRowColor;
    }

    private Color GetTextColor(TaskRowState state)
    {
        if (state == TaskRowState.MustDo) return mustDoTextColor;
        if (state == TaskRowState.Finished) return finishedTextColor;
        return missedTextColor;
    }
}
