using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Object/Sounds/Sound Effect")]
public class SoundEffectSO : ScriptableObject
{
    public string soundEffectName;
    public GameObject soundEffectPrefab;
    public AudioClip audioClip;

    [Range(0.1f, 1.5f)] public float pitchVariantMin = 0.8f;
    [Range(0.1f, 1.5f)] public float pitchVariantMax = 1.2f;
    [Range(0f, 1.0f)] public float volume = 1f;

    [HorizontalGroup("Split", 0.5f)]
    [Button(ButtonSizes.Large)]
    public void Play()
    {
        if (SoundEffectManager.Instance == null)
        {
            return;
        }

        SoundEffectManager.Instance.PlaySoundEffect(this);
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(soundEffectName), soundEffectName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(soundEffectPrefab), soundEffectPrefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(audioClip), audioClip);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(pitchVariantMin), pitchVariantMin, nameof(pitchVariantMax), pitchVariantMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(volume), volume, false);
    }
#endif
    #endregion
}