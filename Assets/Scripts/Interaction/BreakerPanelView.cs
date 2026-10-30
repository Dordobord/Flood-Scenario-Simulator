using UnityEngine;

public class BreakerPanelView : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private Transform[] switches;
    [Tooltip("Points out of the panel, toward the room.")]
    [SerializeField] private Transform front;

    private Quaternion[] switchesOn;
    private bool switchesOff;

    private void Start()
    {
        switchesOn = new Quaternion[switches != null ? switches.Length : 0];
        for (int i = 0; i < switchesOn.Length; i++)
        {
            if (switches[i] != null) switchesOn[i] = switches[i].rotation;
        }

        ShowSwitches(!PowerSupply.HasPower);
    }

    private void Update()
    {
        bool powerOff = !PowerSupply.HasPower;
        if (powerOff != switchesOff) ShowSwitches(powerOff);
    }

    private void ShowSwitches(bool off)
    {
        switchesOff = off;
        Vector3 frontDirection = front != null ? front.forward : transform.forward;

        for (int i = 0; i < switchesOn.Length; i++)
        {
            if (switches[i] == null) continue;
            switches[i].rotation = off ? Quaternion.AngleAxis(180f, frontDirection) * switchesOn[i] : switchesOn[i];
        }
    }
}
