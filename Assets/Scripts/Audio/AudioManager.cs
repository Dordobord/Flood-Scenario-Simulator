using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Library")]
    [SerializeField] private SoundLibrary library;

    [Header("Mixer")]
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup ambienceGroup;

    [Header("Object Loops")]
    [SerializeField] private float loopMinDistance = 1f;
    [SerializeField] private float loopMaxDistance = 15f;

    private AudioSource sfxSource;
    private AudioSource[] musicSources = new AudioSource[2];
    private int activeMusic;
    private Coroutine musicFade;
    private Dictionary<AudioClip, AudioSource> loops = new Dictionary<AudioClip, AudioSource>();
    private Dictionary<Transform, AudioSource> objectLoops = new Dictionary<Transform, AudioSource>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (library != null) SoundLibrary.Current = library;

        sfxSource = CreateSource(sfxGroup);
        musicSources[0] = CreateSource(musicGroup);
        musicSources[1] = CreateSource(musicGroup);
        musicSources[0].loop = true;
        musicSources[1].loop = true;
    }

    private IEnumerator Start()
    {
        if (TaskManager.Instance != null) TaskManager.Instance.TaskCompleted += OnTaskCompleted;
        GameEvents.PreparationEnded += OnPreparationEnded;

        yield return null;
        HookButtons();
    }

    private void OnDestroy()
    {
        if (TaskManager.Instance != null) TaskManager.Instance.TaskCompleted -= OnTaskCompleted;
        GameEvents.PreparationEnded -= OnPreparationEnded;

        if (Instance == this) Instance = null;
    }

    public static void PlaySfx(AudioClip clip)
    {
        if (Instance == null || clip == null) return;
        Instance.sfxSource.PlayOneShot(clip, SoundLibrary.Current.sfxVolume);
    }

    public static void PlayRandom(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        PlaySfx(clips[Random.Range(0, clips.Length)]);
    }

    public static void PlayMusic(AudioClip clip)
    {
        if (Instance == null || clip == null) return;
        Instance.FadeToMusic(clip);
    }

    public static void StopMusic()
    {
        if (Instance == null) return;
        Instance.FadeToMusic(null);
    }

    public static void PlayLoop(AudioClip clip)
    {
        if (Instance == null || clip == null) return;
        Instance.StartLoop(clip);
    }

    public static void StopLoop(AudioClip clip)
    {
        if (Instance == null || clip == null) return;

        AudioSource source;
        if (Instance.loops.TryGetValue(clip, out source)) source.Stop();
    }

    public static void PlayLoopAt(AudioClip clip, Transform target)
    {
        if (Instance == null || clip == null || target == null) return;
        Instance.StartLoopAt(clip, target);
    }

    public static void StopLoopAt(Transform target)
    {
        if (Instance == null || target == null) return;

        AudioSource source;
        if (Instance.objectLoops.TryGetValue(target, out source) && source != null) source.Stop();
    }

    private AudioSource CreateSource(AudioMixerGroup group)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.outputAudioMixerGroup = group;
        return source;
    }

    private void StartLoop(AudioClip clip)
    {
        AudioSource source;
        if (!loops.TryGetValue(clip, out source))
        {
            source = CreateSource(ambienceGroup);
            source.clip = clip;
            source.loop = true;
            loops[clip] = source;
        }

        source.volume = SoundLibrary.Current.ambienceVolume;
        if (!source.isPlaying) source.Play();
    }

    private void StartLoopAt(AudioClip clip, Transform target)
    {
        AudioSource source;
        if (!objectLoops.TryGetValue(target, out source) || source == null)
        {
            GameObject holder = new GameObject("Loop_" + clip.name);
            holder.transform.SetParent(target, false);

            source = holder.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = true;
            source.spatialBlend = 1f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.minDistance = loopMinDistance;
            source.maxDistance = loopMaxDistance;
            source.outputAudioMixerGroup = sfxGroup;
            objectLoops[target] = source;
        }

        source.clip = clip;
        source.volume = SoundLibrary.Current.sfxVolume;
        if (!source.isPlaying) source.Play();
    }

    private void FadeToMusic(AudioClip clip)
    {
        AudioSource current = musicSources[activeMusic];
        if (clip != null && current.clip == clip && current.isPlaying) return;

        if (musicFade != null) StopCoroutine(musicFade);
        musicFade = StartCoroutine(MusicFade(clip));
    }

    private IEnumerator MusicFade(AudioClip clip)
    {
        AudioSource fadingOut = musicSources[activeMusic];
        activeMusic = 1 - activeMusic;
        AudioSource fadingIn = musicSources[activeMusic];

        float targetVolume = SoundLibrary.Current.musicVolume;
        float seconds = Mathf.Max(0.01f, SoundLibrary.Current.musicFadeSeconds);
        float startVolume = fadingOut.volume;

        if (clip != null)
        {
            fadingIn.clip = clip;
            fadingIn.volume = 0f;
            fadingIn.Play();
        }

        float elapsed = 0f;
        while (elapsed < seconds)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / seconds;
            fadingOut.volume = Mathf.Lerp(startVolume, 0f, t);
            if (clip != null) fadingIn.volume = Mathf.Lerp(0f, targetVolume, t);
            yield return null;
        }

        fadingOut.Stop();
        musicFade = null;
    }

    private void HookButtons()
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include);
        foreach (Button button in buttons)
        {
            if (button.GetComponentInParent<MinigameBase>(true) != null) continue;
            button.onClick.AddListener(PlayButtonClick);
        }
    }

    private void PlayButtonClick()
    {
        PlaySfx(SoundLibrary.Current.buttonClick);
    }

    private void OnTaskCompleted(TaskData task)
    {
        PlaySfx(SoundLibrary.Current.taskComplete);
    }

    private void OnPreparationEnded()
    {
        SoundLibrary sounds = SoundLibrary.Current;
        PlaySfx(sounds.evacSiren);
        PlayLoop(sounds.floodWater);
    }
}
