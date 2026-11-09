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
    private void Start()
    {
        InitializeResolutionDropdown();
        SetVolumeSliders();
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

    private void SetVolumeSliders()
    {
        if (!PlayerPrefs.HasKey("MasterVolume"))
            PlayerPrefs.SetFloat("MasterVolume", 1f);

        if (!PlayerPrefs.HasKey("MusicVolume"))
            PlayerPrefs.SetFloat("MusicVolume", 1f);

        if (!PlayerPrefs.HasKey("AmbienceVolume"))
            PlayerPrefs.SetFloat("AmbienceVolume", 1f);

        if (!PlayerPrefs.HasKey("EffectsVolume"))
            PlayerPrefs.SetFloat("EffectsVolume", 1f);

        PlayerPrefs.Save();

        LoadVolume();
    }
    public void ChangeMasterVolume()
    {
        audioMixer.SetFloat("MasterVolume", MasterSlider.value);
    }

    public void ChangeMusicVolume()
    {
        audioMixer.SetFloat("MusicVolume", MusicSlider.value);
    }

    public void ChangeAmbienceVolume()
    {
        audioMixer.SetFloat("AmbienceVolume", AmbienceSlider.value);
    }

    public void ChangeEffectsVolume()
    {
        audioMixer.SetFloat("EffectsVolume", EffectsSlider.value);
    }

    private void LoadVolume()
    {
        MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume");
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        AmbienceSlider.value = PlayerPrefs.GetFloat("AmbienceVolume");
        EffectsSlider.value = PlayerPrefs.GetFloat("EffectsVolume");
    }

    private void SaveVolume()
    {
        PlayerPrefs.SetFloat("MasterVolume", MasterSlider.value);
        PlayerPrefs.SetFloat("MusicVolume", MusicSlider.value);
        PlayerPrefs.SetFloat("AmbienceVolume", AmbienceSlider.value);
        PlayerPrefs.SetFloat("EffectsVolume", EffectsSlider.value);
    }
}
