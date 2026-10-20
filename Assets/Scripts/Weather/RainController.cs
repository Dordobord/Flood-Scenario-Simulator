using System.Collections.Generic;
using UnityEngine;

public class RainController : MonoBehaviour
{
    private const string WetnessProperty = "_RainWetness";
    private const string IntensityProperty = "_RainIntensity";
    private const string SkyExposureProperty = "_Exposure";

    [SerializeField] private ParticleSystem rain;
    [SerializeField] private Light sun;
    [SerializeField] private AudioSource rainSound;
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 12f, 0f);

    private Transform target;
    private ScenarioData scenario;
    private Material skyMaterial;
    private List<Collider> shelters = new List<Collider>();
    private float skyStartExposure = 1f;
    private float sunStartIntensity = 1f;
    private float strength;
    private float targetStrength;
    private float indoor;

    private void Start()
    {
        if (Camera.main != null) target = Camera.main.transform;
        if (TaskManager.Instance != null) scenario = TaskManager.Instance.CurrentScenario;
        if (sun != null) sunStartIntensity = sun.intensity;

        if (RenderSettings.skybox != null)
        {
            skyMaterial = new Material(RenderSettings.skybox);
            RenderSettings.skybox = skyMaterial;
            if (skyMaterial.HasProperty(SkyExposureProperty)) skyStartExposure = skyMaterial.GetFloat(SkyExposureProperty);
        }

        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;

        SetUpShelters();

        targetStrength = scenario != null ? scenario.prepRainStrength : 0f;
        strength = targetStrength;
        indoor = IsSheltered() ? 1f : 0f;
        Apply();

        GameEvents.PreparationEnded += OnPreparationEnded;
    }

    private void OnDestroy()
    {
        GameEvents.PreparationEnded -= OnPreparationEnded;
        Shader.SetGlobalFloat(WetnessProperty, 0f);
        Shader.SetGlobalFloat(IntensityProperty, 0f);
    }

    private void SetUpShelters()
    {
        if (rain == null) return;

        ParticleSystem.TriggerModule trigger = rain.trigger;
        trigger.enabled = true;
        trigger.inside = ParticleSystemOverlapAction.Kill;
        trigger.outside = ParticleSystemOverlapAction.Ignore;
        trigger.enter = ParticleSystemOverlapAction.Kill;
        trigger.exit = ParticleSystemOverlapAction.Ignore;

        foreach (RainShelter shelter in FindObjectsByType<RainShelter>(FindObjectsSortMode.None))
        {
            Collider area = shelter.Area;
            if (area == null) continue;

            shelters.Add(area);
            trigger.AddCollider(area);
        }
    }

    private void OnPreparationEnded()
    {
        if (scenario != null) targetStrength = scenario.evacuationRainStrength;
    }

    private void Update()
    {
        GameSettings settings = GameSettings.Current;
        bool changed = false;

        if (!Mathf.Approximately(strength, targetStrength))
        {
            float rampSeconds = Mathf.Max(0.01f, settings.rainRampSeconds);
            strength = Mathf.MoveTowards(strength, targetStrength, Time.deltaTime / rampSeconds);
            changed = true;
        }

        float targetIndoor = IsSheltered() ? 1f : 0f;
        if (!Mathf.Approximately(indoor, targetIndoor))
        {
            float blendSeconds = Mathf.Max(0.01f, settings.indoorBlendSeconds);
            indoor = Mathf.MoveTowards(indoor, targetIndoor, Time.deltaTime / blendSeconds);
            changed = true;
        }

        if (changed) Apply();
    }

    private void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + followOffset;
    }

    private bool IsSheltered()
    {
        if (target == null) return false;

        foreach (Collider area in shelters)
        {
            if (area != null && area.bounds.Contains(target.position)) return true;
        }
        return false;
    }

    private void Apply()
    {
        GameSettings settings = GameSettings.Current;

        if (rain != null)
        {
            ParticleSystem.EmissionModule emission = rain.emission;
            emission.rateOverTime = settings.maxRainDropsPerSecond * strength;
        }

        float fogDensity = Mathf.Lerp(settings.clearFogDensity, settings.stormFogDensity, strength);
        RenderSettings.fogColor = Color.Lerp(settings.clearFogColor, settings.stormFogColor, strength);
        RenderSettings.fogDensity = fogDensity * Mathf.Lerp(1f, settings.indoorFogMultiplier, indoor);

        if (sun != null) sun.intensity = sunStartIntensity * Mathf.Lerp(1f, settings.stormSunMultiplier, strength);

        if (skyMaterial != null && skyMaterial.HasProperty(SkyExposureProperty))
        {
            skyMaterial.SetFloat(SkyExposureProperty, skyStartExposure * Mathf.Lerp(1f, settings.stormSkyMultiplier, strength));
        }

        Shader.SetGlobalFloat(WetnessProperty, strength * settings.maxWetness);
        Shader.SetGlobalFloat(IntensityProperty, strength);

        if (rainSound != null && rainSound.clip != null)
        {
            rainSound.volume = strength * settings.rainVolume * Mathf.Lerp(1f, settings.indoorRainVolumeMultiplier, indoor);
            if (!rainSound.isPlaying) rainSound.Play();
        }
    }
}
