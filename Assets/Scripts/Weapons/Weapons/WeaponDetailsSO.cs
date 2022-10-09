using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Object/Weapon/Weapon Detail")]
public class WeaponDetailsSO : ScriptableObject
{

    [Space(10)]
    [Header("WEAPON BASE DETAILS")]
    public string weaponName;
    public Sprite weaponSprite;

    [Space(10)]
    [Header("WEAPON CONFIGURATION")]
    public Vector3 shootPosition;
    public AmmoDetailsSO ammo;

    [Space(10)]
    [Header("WEAPON OPERATION DETAILS")]
    public bool hasInfiniteAmmo = false;
    public int ammoCapacity = 100;
    public bool hasInfiniteClipCapacity = false;
    public int ammoClipCapacity = 6;
    public float fireRate = 0.2f;
    public float prechargeTime = 0f;
    public float reloadTime = 0f;

    [Space(10)]
    [Header("WEAPON SOUND EFFECTS")]
    public SoundEffectSO fireSoundEffect;
    public SoundEffectSO reloadSoundEffect;

    [Space(10)]
    [Header("WEAPON VISUAL EFFECTS")]
    public WeaponShootEffectSO fireVisualEffect;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(weaponName), weaponName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammo), ammo);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(fireRate), fireRate, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(prechargeTime), prechargeTime, true);

        if (!hasInfiniteAmmo)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoCapacity), ammoCapacity, false);
        }

        if (!hasInfiniteClipCapacity)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoClipCapacity), ammoClipCapacity, false);
        }
    }
#endif
    #endregion
}