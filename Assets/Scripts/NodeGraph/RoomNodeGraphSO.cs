using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Object/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO typeList;
    [HideInInspector] public List<RoomNodeSO> nodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> nodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void Awake()
    {
        LoadListToDictionary();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        LoadListToDictionary();
    }
#endif

    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    public void LoadListToDictionary()
    {
        nodeDictionary.Clear();

        foreach (var node in nodeList)
        {
            nodeDictionary.Add(node.id, node);
        }
    }

    public RoomNodeSO GetRoomNode(RoomNodeTypeSO type)
    {
        foreach (var node in nodeList)
        {
            if (node.type == type)
            {
                return node;
            }
        }

        return null;
    }

    public List<RoomNodeSO> GetChildRoomNode(RoomNodeSO parent)
    {
        var children = new List<RoomNodeSO>();

        foreach (var id in parent.childIdList)
        {
            var child = GetRoomNode(id);
            if (child != null)
            {
                children.Add(child);
            }
        }

        return children;
    }

    public RoomNodeSO GetRoomNode(string id)
    {
        if (nodeDictionary.TryGetValue(id, out RoomNodeSO roomNode))
        {
            return roomNode;
        }
        return null;
    }
}