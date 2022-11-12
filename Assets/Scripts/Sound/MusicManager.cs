using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : SingletonAbstract<MusicManager>
{
    [SerializeField] private int volume = 10;
    public int Volume { get { return volume; } }

    private AudioSource _audioSource = null;
    private AudioClip _currentAudioClip = null;

    private Coroutine _fadeInRoutine;
    private Coroutine _fadeOutRoutine;


    private void Awake()
    {
        base.Awake();

        _audioSource = GetComponent<AudioSource>();

        GameResources.Instance.musicOffSnapshot.TransitionTo(0f);
    }

    private void Start()
    {
        volume = PlayerPrefs.GetInt("musicVolume", volume);

        SetVolume(volume);
    }

    private void OnDisable()
    {
        PlayerPrefs.SetInt("musicVolume", volume);
    }

    public void PlayMusic(MusicTrackSO track, float fadeIn, float fadeOut)
    {
        StartCoroutine(PlayMusicRoutine(track, fadeIn, fadeOut));
    }

    public void IncreaseVolume()
    {
        if (volume >= 20)
        {
            return;
        }

        volume += 1;
        SetVolume(volume);
    }

    public void DecreaseVolume()
    {
        if (volume <= 0)
        {
            return;
        }

        volume -= 1;
        SetVolume(volume);
    }

    private IEnumerator PlayMusicRoutine(MusicTrackSO track, float fadeIn, float fadeOut)
    {
        if (track.musicClip == _currentAudioClip)
        {
            yield break;
        }

        if (_fadeInRoutine != null)
        {
            StopCoroutine(_fadeInRoutine);
        }

        if (_fadeOutRoutine != null)
        {
            StopCoroutine(_fadeOutRoutine);
        }

        _currentAudioClip = track.musicClip;

        yield return _fadeOutRoutine = StartCoroutine(FadeOutMusic(fadeOut));
        yield return _fadeInRoutine = StartCoroutine(FadeInMusic(track, fadeIn));

        yield return null;
    }

    private IEnumerator FadeInMusic(MusicTrackSO track, float time)
    {
        _audioSource.clip = track.musicClip;
        _audioSource.volume = track.musicVolume;
        _audioSource.Play();

        GameResources.Instance.musicOnFullSnapshot.TransitionTo(time);

        yield return new WaitForSeconds(time);
    }

    private IEnumerator FadeOutMusic(float time)
    {
        GameResources.Instance.musicLowSnapshot.TransitionTo(time);

        yield return new WaitForSeconds(time);
    }

    private void SetVolume(int volume)
    {
        float muteDecibel = -80f;

        if (volume <= 0)
        {
            GameResources.Instance.musicMasterAudioMixer.audioMixer.SetFloat("musicVolume", muteDecibel);
        }
        else
        {
            GameResources.Instance.musicMasterAudioMixer.audioMixer.SetFloat("musicVolume", HelperUtilities.LinearToDecibels(volume));
        }
    }
}
