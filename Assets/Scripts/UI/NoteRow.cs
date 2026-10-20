using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NoteRow : MonoBehaviour
{
    [Header("Task")]
    [SerializeField] private TaskData task;

    [Header("Parts")]
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image box;
    [SerializeField] private Image tick;

    private Color labelColor;
    private Color boxColor;
    private bool hasDesignColors;

    private void OnValidate()
    {
        if (task != null && label != null) label.text = task.title;
    }

    public void Refresh()
    {
        TaskManager tasks = TaskManager.Instance;
        bool inScenario = task != null && tasks != null && tasks.ActiveTasks.Contains(task);

        gameObject.SetActive(inScenario);
        if (!inScenario) return;

        if (!hasDesignColors)
        {
            labelColor = label.color;
            boxColor = box.color;
            hasDesignColors = true;
        }

        GameSettings settings = GameSettings.Current;
        bool done = tasks.IsCompleted(task);

        label.text = task.title;
        label.color = done ? settings.noteDoneTextColor : labelColor;
        box.color = done ? settings.noteBoxDoneColor : boxColor;
        tick.enabled = done;
    }
}
