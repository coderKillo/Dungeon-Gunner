using UnityEngine;
using UnityEditor;

public enum RoomNodeTypeStyle
{
    Node1,
    Node2,
    Node3,
    Node4,
    Node5,
    Node6,
}

[CreateAssetMenu(fileName = "RoomNodeType", menuName = "Scriptable Object/Dungeon/Room Node Type")]
public class RoomNodeTypeSO : ScriptableObject
{
    public string typeName;
    public RoomNodeTypeStyle style = RoomNodeTypeStyle.Node1;
    public bool displayInNodeGraphEditor = true;
    public bool isCorridor;
    public bool isCorridorNS;
    public bool isCorridorEW;
    public bool isEntrance;
    public bool isBossRoom;
    public bool isNone;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(typeName), typeName);
    }

    public string GetStyleTextureName()
    {
        switch (style)
        {
            case RoomNodeTypeStyle.Node1:
                return "node1";
            case RoomNodeTypeStyle.Node2:
                return "node2";
            case RoomNodeTypeStyle.Node3:
                return "node3";
            case RoomNodeTypeStyle.Node4:
                return "node4";
            case RoomNodeTypeStyle.Node5:
                return "node5";
            case RoomNodeTypeStyle.Node6:
                return "node6";
            default:
                return "node1";
        }
    }
#endif
}
