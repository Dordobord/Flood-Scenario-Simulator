using UnityEngine;

public class StormSounds : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RainController rain;
    [SerializeField] private AudioSource windSound;
    [SerializeField] private AudioSource heavyWindSound;
    [SerializeField] private AudioSource thunderSound;

    private bool evacuating;
    private float nextThunderTime;

    private void Start()
    {
        PlayWindClip(windSound, SoundLibrary.Current.windLight);
        GameEvents.PreparationEnded += OnPreparationEnded;
    }

    private void OnDestroy()
    {
        GameEvents.PreparationEnded -= OnPreparationEnded;
    }

    private void OnPreparationEnded()
    {
        evacuating = true;
        PlayWindClip(heavyWindSound, SoundLibrary.Current.windHeavy);
        ScheduleThunder();
    }

    private void Update()
    {
        UpdateWindVolume();

        if (evacuating && Time.time >= nextThunderTime) PlayThunder();
    }

    private void PlayWindClip(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null) return;

        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    private void UpdateWindVolume()
    {
        if (rain == null) return;

        GameSettings settings = GameSettings.Current;
        float indoorLevel = Mathf.Lerp(1f, settings.indoorWindVolumeMultiplier, rain.IndoorBlend);
        float windLevel = rain.Strength * settings.windVolume * indoorLevel;

        if (windSound != null) windSound.volume = windLevel * (1f - rain.HeavyBlend);
        if (heavyWindSound != null) heavyWindSound.volume = windLevel * rain.HeavyBlend;
    }

    private void PlayThunder()
    {
        ScheduleThunder();

        AudioClip[] clips = SoundLibrary.Current.thunder;
        if (thunderSound == null || clips == null || clips.Length == 0) return;

        GameSettings settings = GameSettings.Current;
        float indoorBlend = rain != null ? rain.IndoorBlend : 0f;
        float volume = settings.thunderVolume * Mathf.Lerp(1f, settings.indoorThunderVolumeMultiplier, indoorBlend);
        thunderSound.PlayOneShot(clips[Random.Range(0, clips.Length)], volume);
    }

    private void ScheduleThunder()
    {
        GameSettings settings = GameSettings.Current;
        nextThunderTime = Time.time + Random.Range(settings.thunderMinSeconds, settings.thunderMaxSeconds);
    }
}
