using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : SingletonAbstract<SoundEffectManager>
{
    private int volume = 8;

    void Start()
    {
        SetVolume(volume);
    }

    public void PlaySoundEffect(SoundEffectSO soundEffect)
    {
        var sound = (SoundEffect)PoolManager.Instance.ReuseComponent(soundEffect.soundEffectPrefab, Vector3.zero, Quaternion.identity);
        sound.SetSound(soundEffect);

        StartCoroutine(PlaySoundEffectRoutine(sound, soundEffect.audioClip.length));
    }

    private IEnumerator PlaySoundEffectRoutine(SoundEffect sound, float duration)
    {
        sound.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        sound.gameObject.SetActive(false);
    }

    private void SetVolume(int volume)
    {
        float muteDecibel = -80f;

        float volumeDecibel = HelperUtilities.LinearToDecibels(volume);
        volumeDecibel = (volume <= 0) ? muteDecibel : volumeDecibel;

        GameResources.Instance.soundMasterAudioMixer.audioMixer.SetFloat("soundsVolume", volumeDecibel);
    }
}
