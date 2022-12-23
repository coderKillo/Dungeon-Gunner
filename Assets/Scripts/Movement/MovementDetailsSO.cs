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
    [Header("DASHING")]

    public float dashSpeed;
    public AnimationCurve dashSpeedMultiplier;
    public float dashTime;
    public float dashCooldown;
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

        if (dashCooldown != 0f || dashTime != 0f || dashSpeed != 0f)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(dashCooldown), dashCooldown, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(dashTime), dashTime, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(dashSpeed), dashSpeed, false);
        }
    }
#endif
    #endregion
}