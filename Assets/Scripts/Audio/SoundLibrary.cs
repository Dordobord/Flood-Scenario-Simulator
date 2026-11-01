using UnityEngine;

[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Storm Waits/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Header("Volume")]
    [Range(0f, 1f)] public float musicVolume = 0.6f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float ambienceVolume = 0.8f;
    public float musicFadeSeconds = 1.5f;

    [Header("Ambience")]
    public AudioClip rainLight;
    public AudioClip rainHeavy;
    public AudioClip rainLightIndoor;
    public AudioClip rainHeavyIndoor;
    public AudioClip windLight;
    public AudioClip windHeavy;
    public AudioClip[] thunder;
    public AudioClip floodWater;
    public AudioClip evacCenterAmbience;

    [Header("Music")]
    public AudioClip menuMusic;

    [Header("Player")]
    public AudioClip potholeSplash;
    public AudioClip flashlightOn;
    public AudioClip flashlightOff;

    [Header("House")]
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public AudioClip windowOpen;
    public AudioClip windowClose;
    public AudioClip drawerOpen;
    public AudioClip drawerClose;
    public AudioClip gateCreak;
    public AudioClip powerOn;

    [Header("Tasks")]
    public AudioClip breakerCover;
    public AudioClip breakerFlip;
    public AudioClip spark;
    public AudioClip unplug;
    public AudioClip stoveCrank;
    public AudioClip battery;
    public AudioClip itemDrop;
    public AudioClip goBagPlace;
    public AudioClip goBagZip;
    public AudioClip tvStatic;
    public AudioClip tvClick;
    public AudioClip radioStatic;
    public AudioClip sandbagThud;
    public AudioClip plugInBeep;

    [Header("UI")]
    public AudioClip buttonClick;
    public AudioClip taskComplete;
    public AudioClip fail;
    public AudioClip timerBeep;
    public AudioClip evacSiren;
    public AudioClip arrivalChime;

    private static SoundLibrary current;

    public static SoundLibrary Current
    {
        get
        {
            if (current == null) current = CreateInstance<SoundLibrary>();
            return current;
        }
        set
        {
            current = value;
        }
    }
}
