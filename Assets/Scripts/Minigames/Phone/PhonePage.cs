using UnityEngine;

public class PhonePage : MonoBehaviour
{
    [Header("Task")]
    [SerializeField] private TaskData task;

    protected PhoneMinigame Phone { get; private set; }

    public TaskData Task
    {
        get { return task; }
    }

    public void Init(PhoneMinigame phone)
    {
        Phone = phone;
    }

    public virtual void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
