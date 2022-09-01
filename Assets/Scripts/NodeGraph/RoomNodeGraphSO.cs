using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Object/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO typeList;
    [HideInInspector] public List<RoomNodeSO> nodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> nodeDictionary = new Dictionary<string, RoomNodeSO>();

#if UNITY_EDITOR
    private void Awake()
    {
        LoadListToDictionary();
    }

    private void OnValidate()
    {
        LoadListToDictionary();
    }

    [HideInInspector] public RoomNodeSO roomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    public void LoadListToDictionary()
    {
        nodeDictionary.Clear();

        foreach (var roomNode in nodeList)
        {
            nodeDictionary.Add(roomNode.id, roomNode);
        }
    }

    public RoomNodeSO GetRoomNode(string id)
    {
        if (nodeDictionary.TryGetValue(id, out RoomNodeSO roomNode))
        {
            return roomNode;
        }
        return null;
    }

#endif
}