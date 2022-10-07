using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class SoundEffect : MonoBehaviour
{

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }

    public void SetSound(SoundEffectSO soundEffect)
    {
        audioSource.clip = soundEffect.audioClip;
        audioSource.volume = soundEffect.volume;
        audioSource.pitch = Random.Range(soundEffect.pitchVariantMin, soundEffect.pitchVariantMax);
    }

}
