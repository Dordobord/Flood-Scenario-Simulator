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

    [Header("Done Colors")]
    [SerializeField] private Color doneTextColor = new Color(0.373f, 0.369f, 0.353f);
    [SerializeField] private Color doneBoxColor = new Color(0.173f, 0.173f, 0.165f);

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

        bool done = tasks.IsCompleted(task);

        label.text = task.title;
        label.color = done ? doneTextColor : labelColor;
        box.color = done ? doneBoxColor : boxColor;
        tick.enabled = done;
    }
}
