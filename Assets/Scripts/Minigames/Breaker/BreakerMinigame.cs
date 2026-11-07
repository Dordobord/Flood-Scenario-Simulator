using System.Collections.Generic;
using UnityEngine;

public class BreakerMinigame : MinigameBase
{
    [Header("References")]
    [SerializeField] private Transform circuitParent;
    [SerializeField] private BreakerSwitch mainSwitch;

    [Header("Warning")]
    [SerializeField] private float warningSeconds = 0.4f;

    private List<BreakerSwitch> circuits = new List<BreakerSwitch>();
    private float warningTimer;

    public override AudioClip MistakeSound
    {
        get { return SoundLibrary.Current.spark; }
    }

    protected override void Setup()
    {
        AudioManager.PlaySfx(SoundLibrary.Current.breakerCover);
        circuits.Clear();
        circuitParent.GetComponentsInChildren(true, circuits);

        for (int i = 0; i < circuits.Count; i++)
        {
            string label = i < settings.choices.Count ? settings.choices[i].label : "";
            circuits[i].Setup(label, OnCircuitClicked);
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
