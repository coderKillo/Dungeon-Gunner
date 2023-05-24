using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDetails_", menuName = "Scriptable Object/Player/Player Details")]
public class PlayerDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("BASE DETAILS")]
    public string characterName;
    public GameObject prefab;
    public RuntimeAnimatorController runtimeAnimatorController;

    [Space(10)]
    [Header("HEALTH")]
    public int healthAmount;
    public float hitImmunityTime = 0f;

    [Space(10)]
    [Header("WEAPON")]
    public CardSO startingWeapon;

    [Space(10)]
    [Header("OTHER")]
    public Sprite minimapIcon;
    public Sprite handSprite;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(characterName), characterName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(healthAmount), healthAmount, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapIcon), minimapIcon);
        HelperUtilities.ValidateCheckNullValue(this, nameof(handSprite), handSprite);
        HelperUtilities.ValidateCheckNullValue(this, nameof(runtimeAnimatorController), runtimeAnimatorController);
        HelperUtilities.ValidateCheckNullValue(this, nameof(startingWeapon), startingWeapon);
    }
#endif
    #endregion
}