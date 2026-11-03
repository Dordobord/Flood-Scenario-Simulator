using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    Resolution[] resolutions;
    public TMP_Dropdown resolutionDropdown;

    [SerializeField] private Slider MasterSlider;
    [SerializeField] private Slider MusicSlider;
    [SerializeField] private Slider AmbienceSlider;
    [SerializeField] private Slider EffectsSlider;
    public AudioMixer audioMixer;
    public SoundLibrary soundLibrary;
    void Start()
    {
        InitializeResolutionDropdown();

        SetVolumeSliderValue();
    }

    private void InitializeResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetFullScreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetVolume()
    {
        float masterVolume = MasterSlider.value;
        float musicVolume = MusicSlider.value;
        float ambienceVolume = AmbienceSlider.value;
        float effectsVolume = EffectsSlider.value;
        audioMixer.SetFloat("MasterVolume", masterVolume);
        audioMixer.SetFloat("MusicVolume", musicVolume);
        audioMixer.SetFloat("AmbienceVolume", ambienceVolume);
        audioMixer.SetFloat("EffectsVolume", effectsVolume);
    }

    private void SetVolumeSliderValue()
    {
        float defaultVolume;
        bool result = audioMixer.GetFloat("volume", out defaultVolume);

        if (result)
        {
            MasterSlider.value = defaultVolume;
        }
    }

    private void OnEnable()
    {
        SetVolumeSliderValue();
    }

    //back button
    public void Back()
    {
        SceneManager.LoadScene("MASTER_Start");
    }
}
