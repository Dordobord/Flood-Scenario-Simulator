using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskRowUI : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text label;

    public void Show(TaskData task, TaskRowState state)
    {
        GameSettings settings = GameSettings.Current;

        label.text = task.ResultName;
        label.color = settings.GetTaskRowTextColor(state);
        background.enabled = state == TaskRowState.MustDo;
        background.color = settings.mustDoRowColor;
    }
}
