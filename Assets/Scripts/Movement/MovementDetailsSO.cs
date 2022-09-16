using UnityEngine;

[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Object/Movement/Movement Details")]
public class MovementDetailsSO : ScriptableObject
{
    #region HEADER
    [Space(10)]
    [Header("MOVEMENT DETAILS")]

    public float minMoveSpeed = 8f;
    public float maxMoveSpeed = 8f;
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
    }
#endif
    #endregion
}