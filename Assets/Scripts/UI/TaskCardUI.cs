using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskCardUI : MonoBehaviour
{
    [Header("Domain")]
    [SerializeField] private PreparednessDomain domain;

    [Header("Parts")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Transform rowParent;

    [Header("Prefabs")]
    [SerializeField] private TaskRowUI rowPrefab;

    public PreparednessDomain Domain
    {
        get { return domain; }
    }

    public void Show(List<TaskData> tasks)
    {
        DomainDisplayName display = GameSettings.Current.GetDomainDisplay(domain);
        if (display == null) return;

        iconImage.sprite = display.icon;
        iconImage.enabled = display.icon != null;
        nameText.text = display.shortName;

        ClearRows();
        AddRows(tasks, TaskRowState.MustDo);
        AddRows(tasks, TaskRowState.Finished);
        AddRows(tasks, TaskRowState.Missed);
    }

    private void ClearRows()
    {
        foreach (Transform row in rowParent)
        {
            Destroy(row.gameObject);
        }
    }

    private void AddRows(List<TaskData> tasks, TaskRowState state)
    {
        foreach (TaskData task in tasks)
        {
            if (task == null || task.domain != domain || GetState(task) != state) continue;

            TaskRowUI row = Instantiate(rowPrefab, rowParent);
            row.Show(task, state);
        }
    }

    private TaskRowState GetState(TaskData task)
    {
        TaskResult result;
        if (!TaskManager.Instance.TryGetResult(task, out result)) return TaskRowState.Missed;
        if (task.priority == TaskPriority.Critical) return TaskRowState.MustDo;
        return TaskRowState.Finished;
    }
}
