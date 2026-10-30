using System.Collections.Generic;
using UnityEngine;

public class BreakerMinigame : MinigameBase
{
    [Header("References")]
    [SerializeField] private Transform circuitParent;
    [SerializeField] private BreakerSwitch mainSwitch;

    [Header("Prefabs")]
    [SerializeField] private BreakerSwitch circuitPrefab;

    [Header("Warning")]
    [SerializeField] private float warningSeconds = 0.4f;

    private List<BreakerSwitch> circuits = new List<BreakerSwitch>();
    private float warningTimer;

    protected override void Setup()
    {
        AudioManager.PlaySfx(SoundLibrary.Current.breakerCover);
        ClearChildren(circuitParent);
        circuits.Clear();

        foreach (SelectChoice choice in settings.choices)
        {
            BreakerSwitch circuit = Instantiate(circuitPrefab, circuitParent);
            circuit.Setup(choice.label, OnCircuitClicked);
            circuits.Add(circuit);
        }

        mainSwitch.Setup(GameSettings.Current.breakerMainLabel, OnMainClicked);

        warningTimer = 0f;
    }

    public override void Tick()
    {
        if (warningTimer <= 0f) return;

        warningTimer -= Time.deltaTime;
        if (warningTimer <= 0f) mainSwitch.SetWarning(false);
    }

    private void OnCircuitClicked(BreakerSwitch circuit)
    {
        if (IsFinished || !circuit.IsOn) return;

        circuit.SetOn(false);
        AudioManager.PlaySfx(SoundLibrary.Current.breakerFlip);
    }

    private void OnMainClicked(BreakerSwitch main)
    {
        if (IsFinished) return;

        if (CountCircuitsOn() > 0)
        {
            Mistakes++;
            AudioManager.PlaySfx(SoundLibrary.Current.spark);
            main.SetWarning(true);
            warningTimer = warningSeconds;
            ShowFeedback(GameSettings.Current.breakerMainEarlyHint);
            return;
        }

        main.SetOn(false);
        AudioManager.PlaySfx(SoundLibrary.Current.breakerFlip);
        Finish(true);
    }

    private int CountCircuitsOn()
    {
        int count = 0;
        foreach (BreakerSwitch circuit in circuits)
        {
            if (circuit.IsOn) count++;
        }
        return count;
    }
}
