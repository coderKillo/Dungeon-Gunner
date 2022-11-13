using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _soundLevelText;
    [SerializeField] private TextMeshProUGUI _musicLevelText;

    void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Time.timeScale = 0f;

        StartCoroutine(InitializeUI());
    }

    private IEnumerator InitializeUI()
    {
        yield return null;

        _soundLevelText.text = SoundEffectManager.Instance.Volume.ToString();
        _musicLevelText.text = MusicManager.Instance.Volume.ToString();
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void IncreaseSoundVolume()
    {
        SoundEffectManager.Instance.IncreaseVolume();
        _soundLevelText.text = SoundEffectManager.Instance.Volume.ToString();
    }

    public void DecreaseSoundVolume()
    {
        SoundEffectManager.Instance.DecreaseVolume();
        _soundLevelText.text = SoundEffectManager.Instance.Volume.ToString();
    }

    public void IncreaseMusicVolume()
    {
        MusicManager.Instance.IncreaseVolume();
        _musicLevelText.text = MusicManager.Instance.Volume.ToString();
    }

    public void DecreaseMusicVolume()
    {
        MusicManager.Instance.DecreaseVolume();
        _musicLevelText.text = MusicManager.Instance.Volume.ToString();
    }
}
