using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Object/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject
{
    #region HEADER
    [Space(10)]
    [Header("MOVEMENT")]

    public float minMoveSpeed = 8f;
    public float maxMoveSpeed = 8f;

    [Space(10)]
    [Header("ROLLING")]

    public float rollSpeed;
    public float rollDistance;
    public float rollCooldown;
    #endregion

    public float GetRandomMovementSpeed()
    {
        if (maxMoveSpeed == minMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return UnityEngine.Random.Range(minMoveSpeed, maxMoveSpeed);
        }
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollCooldown != 0f || rollDistance != 0f || rollSpeed != 0f)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCooldown), rollCooldown, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
        }
    }
#endif
    #endregion
}