using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Object/Weapon/Ammo Detail")]
public class AmmoDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("AMMO BASE DETAILS")]
    public string ammoName;
    public bool isPlayerAmmo;

    [Space(10)]
    [Header("AMMO SPRITES, PREFABS & MATERIALS")]
    public Sprite ammoSprite;
    public GameObject[] prefabArray;
    public Material ammoMaterial;

    [Space(10)]
    [Header("AMMO CHARGE")]
    public float chargeTime = 0f;
    public Material chargeMaterial;

    [Space(10)]
    [Header("AMMO BASE PARAMETER")]
    public int damage = 1;
    public float critChance = 0.1f;
    public float speedMin = 20f;
    public float speedMax = 20f;
    public float range = 20f;
    public float rotationSpeed = 0f;

    [Space(10)]
    [Header("AMMO HIT DETAILS")]
    public float damageRadius = 0f;
    public int hitLimit = 0;
    public GameObject onHitPrefab;
    public int onHitDamage = 0;

    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]
    public float spreadMin = 0f;
    public float spreadMax = 0f;

    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]
    public int spawnAmmoMin = 1;
    public int spawnAmmoMax = 1;
    public float spawnIntervalMin = 0f;
    public float spawnIntervalMax = 0f;

    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]
    public bool hasTrail = false;
    public float trailLifetime = 3f;
    public Material trailMaterial;
    [Range(0f, 1f)] public float trailStartWidth;
    [Range(0f, 1f)] public float trailEndWidth;

    [Space(10)]
    [Header("AMMO MELEE DETAILS")]
    public bool isMelee = false;
    [ShowIf("isMelee")] public float startAngle = 45f;
    [ShowIf("isMelee")] public float endAngle = -135f;
    [ShowIf("isMelee")] public float hitboxWidth = 1f;
    [ShowIf("isMelee")] public float rotationDuration = 1f;
    [ShowIf("isMelee")] public float dashDistance = 0f;

    [Space(10)]
    [Header("AMMO VISUAL EFFECTS")]
    public SpriteEffectSO hitVisualEffect;

    [Space(10)]
    [Header("AMMO AUDIO EFFECTS")]
    public SoundEffectSO hitAudioEffect;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(prefabArray), prefabArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);

        if (chargeTime > 0)
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(chargeMaterial), chargeMaterial);
        }

        HelperUtilities.ValidateCheckPositiveValue(this, nameof(damage), damage, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(range), range, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(speedMin), speedMin, nameof(speedMax), speedMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(spreadMin), spreadMin, nameof(spreadMax), spreadMax, true);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(spawnAmmoMin), spawnAmmoMin, nameof(spawnAmmoMax), spawnAmmoMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(spawnIntervalMin), spawnIntervalMin, nameof(spawnIntervalMax), spawnIntervalMax, true);

        if (hasTrail)
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(trailMaterial), trailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(trailLifetime), trailLifetime, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(trailStartWidth), trailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(trailEndWidth), trailEndWidth, false);
        }
    }
#endif
    #endregion
}
