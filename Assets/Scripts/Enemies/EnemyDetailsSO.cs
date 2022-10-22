using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Object/Enemy/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("BASE DETAILS")]
    public string enemyName;
    public GameObject prefab;
    public float chaseDistance = 50f;

    // public RuntimeAnimatorController runtimeAnimatorController;

    // [Space(10)]
    // [Header("HEALTH")]
    // public int healthAmount;

    // [Space(10)]
    // [Header("WEAPON")]
    // public WeaponDetailsSO startingWeapon;
    // public List<WeaponDetailsSO> startingWeaponList;

    // [Space(10)]
    // [Header("OTHER")]
    // public Sprite minimapIcon;
    // public Sprite handSprite;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        // HelperUtilities.ValidateCheckPositiveValue(this, nameof(healthAmount), healthAmount, false);
        // HelperUtilities.ValidateCheckNullValue(this, nameof(minimapIcon), minimapIcon);
        // HelperUtilities.ValidateCheckNullValue(this, nameof(handSprite), handSprite);
        // HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        // HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
        // HelperUtilities.ValidateCheckEnumerableValues(this, nameof(startingWeaponList), startingWeaponList);
    }
#endif
    #endregion
}