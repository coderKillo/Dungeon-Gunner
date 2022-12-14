using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "RoomNodeType", menuName = "Scriptable Object/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string typeName;
    public bool displayInNodeGraphEditor = true;
    public bool isCorridor;
    public bool isCorridorNS;
    public bool isCorridorEW;
    public bool isEntrance;
    public bool isBossRoom;
    public bool isChestRoom;
    public bool isNone;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(typeName), typeName);
    }
#endif
}
