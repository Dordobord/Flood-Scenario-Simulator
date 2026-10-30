using UnityEngine;

public class CeilingLamp : MonoBehaviour
{
    [Header("Parts")]
    [SerializeField] private Light lampLight;
    [SerializeField] private Renderer bulbRenderer;
    [SerializeField] private Material bulbOnMaterial;
    [SerializeField] private Material bulbOffMaterial;

    [Header("Flicker")]
    [SerializeField] private float flickerSeconds = 0.8f;
    [SerializeField] private float flickerInterval = 0.08f;

    private bool hasPower;
    private float flickerTimeLeft;
    private float nextFlickerTime;
    private bool flickerLightOn;

    private void Start()
    {
        hasPower = PowerSupply.HasPower;
        SetLampOn(hasPower);
    }

    private void Update()
    {
        if (PowerSupply.HasPower != hasPower) OnPowerChanged(PowerSupply.HasPower);

        if (flickerTimeLeft > 0f) UpdateFlicker();
    }

    private void OnPowerChanged(bool powerOn)
    {
        hasPower = powerOn;

        if (powerOn)
        {
            flickerTimeLeft = 0f;
            SetLampOn(true);
            return;
        }

        flickerTimeLeft = flickerSeconds;
        flickerLightOn = true;
        nextFlickerTime = Time.time;
    }

    private void UpdateFlicker()
    {
        flickerTimeLeft -= Time.deltaTime;

        if (flickerTimeLeft <= 0f)
        {
            SetLampOn(false);
            return;
        }

        if (Time.time < nextFlickerTime) return;

        flickerLightOn = !flickerLightOn;
        SetLampOn(flickerLightOn);
        nextFlickerTime = Time.time + flickerInterval;
    }

    private void SetLampOn(bool on)
    {
        if (lampLight != null) lampLight.enabled = on;

        if (bulbRenderer == null || bulbOnMaterial == null || bulbOffMaterial == null) return;
        bulbRenderer.sharedMaterial = on ? bulbOnMaterial : bulbOffMaterial;
    }
}
