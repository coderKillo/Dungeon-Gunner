using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Object/Enemy/Enemy Details")]
public class EnemyDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("BASE DETAILS")]
    public string enemyName;
    public GameObject prefab;
    public float chaseDistance = 50f;
    public int value = 1;

    [Space(10)]
    [Header("MATERIAL DETAILS")]
    public Material standardMaterial;

    [Space(10)]
    [Header("MATERIALIZE DETAILS")]
    public float materializeTime;
    [ColorUsage(true, true)] public Color materializeColor;
    public Shader materializeShader;

    [Space(10)]
    [Header("WEAPON DETAILS")]
    public WeaponDetailsSO weaponDetails;
    public float firingIntervalMin = 0.1f;
    public float firingIntervalMax = 1f;
    public float RandomFiringInterval { get { return Random.Range(firingIntervalMin, firingIntervalMax); } }
    public float firingDurationMin = 1f;
    public float firingDurationMax = 2f;
    public float RandomFiringDuration { get { return Random.Range(firingDurationMin, firingDurationMax); } }
    public bool firingLineOfSightRequire;

    [Space(10)]
    [Header("MELEE ATTACKS")]
    public bool hasMeleeAttacks;
    [ShowIf("hasMeleeAttacks")] public float meleeRange = 1f;
    [ShowIf("hasMeleeAttacks")] public float meleeDamage = 10f;

    [Space(10)]
    [Header("HEALTH DETAILS")]
    public int baseHealth;
    public bool isImmuneAfterHit = false;
    public int hitImmuneTime;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(prefab), prefab);
        HelperUtilities.ValidateCheckNullValue(this, nameof(standardMaterial), standardMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(materializeShader), materializeShader);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(materializeTime), materializeTime, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmuneTime), hitImmuneTime, true);
    }
#endif
    #endregion
}