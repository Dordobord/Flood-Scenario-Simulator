using UnityEngine;

public class PowerSupply : MonoBehaviour
{
    public static PowerSupply Instance { get; private set; }

    [Header("Breaker")]
    [SerializeField] private TaskData breakerTask;

    public static bool HasPower
    {
        get
        {
            if (Instance == null || Instance.breakerTask == null || TaskManager.Instance == null) return true;
            return !TaskManager.Instance.IsCompleted(Instance.breakerTask);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    public void TurnPowerBackOn()
    {
        if (HasPower || TaskManager.Instance == null) return;

        TaskManager.Instance.ResetTask(breakerTask);
        GameEvents.RaiseMessage(GameSettings.Current.powerOnMessage);
    }
}
