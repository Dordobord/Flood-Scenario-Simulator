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
    public AudioClip wind;
    public AudioClip[] thunder;
    public AudioClip indoorRain;
    public AudioClip floodWater;

    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip tensionMusic;
    public AudioClip resultsSting;

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
    public AudioClip lampBuzz;
    public AudioClip lampPowerOff;
    public AudioClip powerOn;
    public AudioClip fanHum;

    [Header("Tasks")]
    public AudioClip breakerCover;
    public AudioClip breakerFlip;
    public AudioClip spark;
    public AudioClip unplug;
    public AudioClip stoveKnob;
    public AudioClip lpgValve;
    public AudioClip battery;
    public AudioClip itemDrop;
    public AudioClip goBagPlace;
    public AudioClip goBagZip;
    public AudioClip cardPick;
    public AudioClip tvStatic;
    public AudioClip tvClick;
    public AudioClip radioStatic;
    public AudioClip sandbagThud;
    public AudioClip plugInBeep;
    public AudioClip paper;
    public AudioClip holdTick;

    [Header("UI")]
    public AudioClip buttonClick;
    public AudioClip taskComplete;
    public AudioClip fail;
    public AudioClip hotbarTick;
    public AudioClip toastPop;
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
