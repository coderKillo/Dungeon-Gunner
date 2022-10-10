using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponShootEffect_", menuName = "Scriptable Object/Weapon/Weapon Shoot Effect")]
public class WeaponShootEffectSO : ScriptableObject
{
    public Gradient colorGradient;
    public float duration = 0.5f;
    public float startParticleSize = 0.25f;
    public float startParticleSpeed = 3f;
    public float startLifetime = 0.5f;
    public int maxParticleNumber = 100;
    public int emissionRate = 100;
    public int burstParticleNumber = 20;
    public float effectGravity = -0.01f;
    public Sprite sprite;
    public Vector3 velocityOverLifetimeMin;
    public Vector3 velocityOverLifetimeMax;
    public GameObject prefab;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(sprite), sprite);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(duration), duration, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSize), startParticleSize, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startParticleSpeed), startParticleSpeed, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(startLifetime), startLifetime, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(maxParticleNumber), maxParticleNumber, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(emissionRate), emissionRate, true);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(burstParticleNumber), burstParticleNumber, true);
    }
#endif
    #endregion
}