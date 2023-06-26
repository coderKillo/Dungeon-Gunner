using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Space(10)]
    [Header("References")]
    [SerializeField] private SliderValue _musicSlider;
    [SerializeField] private SliderValue _soundEffectSlider;

    [SerializeField] private Toggle _tutorialToggle;

    void Start()
    {
        _soundEffectSlider.Slider.onValueChanged.AddListener(OnSoundEffectVolumeChanged);
        _soundEffectSlider.Slider.value = PlayerPrefs.GetInt(PrefKeys.soundVolume, Settings.defaultVolume);

        _musicSlider.Slider.onValueChanged.AddListener(OnMusicVolumeChanged);
        _musicSlider.Slider.value = PlayerPrefs.GetInt(PrefKeys.musicVolume, Settings.defaultVolume);

        _tutorialToggle.onValueChanged.AddListener(OnTutorialValueChange);
        _tutorialToggle.isOn = (PlayerPrefs.GetInt(PrefKeys.tutorialOn, 1) == 1);
    }

    private void OnTutorialValueChange(bool on)
    {
        PlayerPrefs.SetInt(PrefKeys.tutorialOn, on ? 1 : 0);
    }

    private void OnSoundEffectVolumeChanged(float volume)
    {
        PlayerPrefs.SetInt(PrefKeys.soundVolume, (int)volume);
    }

    private void OnMusicVolumeChanged(float volume)
    {
        PlayerPrefs.SetInt(PrefKeys.musicVolume, (int)volume);

        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume((int)volume);
        }
    }

    void Update()
    {

    }
}
