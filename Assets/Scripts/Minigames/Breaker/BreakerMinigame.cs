using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BreakerMinigame : MinigameBase
{
    [Header("References")]
    [SerializeField] private Button coverButton;
    [SerializeField] private GameObject switchArea;
    [SerializeField] private Transform circuitParent;
    [SerializeField] private BreakerSwitch mainSwitch;
    [SerializeField] private GameObject spark;
    [SerializeField] private TMP_Text hintText;

    [Header("Prefabs")]
    [SerializeField] private BreakerSwitch circuitPrefab;

    [Header("Spark")]
    [SerializeField] private float sparkSeconds = 0.4f;

    private List<BreakerSwitch> circuits = new List<BreakerSwitch>();
    private float sparkTimer;

    protected override void Setup()
    {
        GameSettings gameSettings = GameSettings.Current;

        ClearChildren(circuitParent);
        circuits.Clear();

        foreach (SelectChoice choice in settings.choices)
        {
            BreakerSwitch circuit = Instantiate(circuitPrefab, circuitParent);
            circuit.Setup(choice.label, OnCircuitClicked);
            circuits.Add(circuit);
        }

        mainSwitch.Setup(gameSettings.breakerMainLabel, OnMainClicked);

        coverButton.onClick.RemoveAllListeners();
        coverButton.onClick.AddListener(OpenCover);
        coverButton.gameObject.SetActive(true);
        switchArea.SetActive(false);

        sparkTimer = 0f;
        if (spark != null) spark.SetActive(false);

        SetHint(gameSettings.breakerCoverHint);
    }

    public override void Tick()
    {
        if (sparkTimer <= 0f) return;

        sparkTimer -= Time.deltaTime;
        if (sparkTimer <= 0f && spark != null) spark.SetActive(false);
    }

    private void OpenCover()
    {
        coverButton.gameObject.SetActive(false);
        switchArea.SetActive(true);
        SetHint(GameSettings.Current.breakerSwitchHint);
    }

    private void OnCircuitClicked(BreakerSwitch circuit)
    {
        if (IsFinished || !circuit.IsOn) return;

        circuit.SetOn(false);
    }

    private void OnMainClicked(BreakerSwitch main)
    {
        if (IsFinished) return;

        if (CountCircuitsOn() > 0)
        {
            Mistakes++;
            ShowSpark();
            SetHint(GameSettings.Current.breakerMainEarlyHint);
            return;
        }

        main.SetOn(false);
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

    private void ShowSpark()
    {
        if (spark == null) return;

        spark.SetActive(true);
        sparkTimer = sparkSeconds;
    }

    private void SetHint(string text)
    {
        if (hintText != null) hintText.text = text;
    }
}
