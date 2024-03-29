using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : SingletonAbstract<SoundEffectManager>
{
    private int volume = 8;
    public int Volume { get { return volume; } }

    private void Start()
    {
        volume = PlayerPrefs.GetInt(PrefKeys.soundVolume, volume);

        SetVolume(volume);
    }

    private void OnDisable()
    {
        Debug.Log("Stop game");
        PlayerPrefs.SetInt(PrefKeys.soundVolume, volume);
    }

    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        var sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundEffectPrefab, Vector3.zero, Quaternion.identity);
        sound.SetSound(soundEffect);

        StartCoroutine(PlaySoundEffectRoutine(sound, soundEffect.audioClip.length));
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

    private IEnumerator PlaySoundEffectRoutine(SoundEffect sound, float duration)
    {
        sound.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        sound.gameObject.SetActive(false);
    }

    public void SetVolume(int volume)
    {
        this.volume = volume;
        float muteDecibel = -80f;

        float volumeDecibel = HelperUtilities.LinearToDecibels(volume);
        volumeDecibel = (volume <= 0) ? muteDecibel : volumeDecibel;

        GameResources.Instance.soundMasterAudioMixer.audioMixer.SetFloat("soundsVolume", volumeDecibel);
    }
}
