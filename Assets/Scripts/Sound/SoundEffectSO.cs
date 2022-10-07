using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "Scriptable Object/Sounds/Sound Effect")]
public class SoundEffectSO : ScriptableObject
{
    public string soundEffectName;
    public GameObject soundEffectPrefab;
    public AudioClip audioClip;

    [Range(0.1f, 1.0f)] public float pitchVariantMin;
    [Range(0.1f, 1.0f)] public float pitchVariantMax;

    public float volume;

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