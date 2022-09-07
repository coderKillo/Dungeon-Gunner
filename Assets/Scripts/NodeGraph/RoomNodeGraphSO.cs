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

    public IEnumerable<RoomNodeSO> GetChildRoomNode(RoomNodeSO parent)
    {
        foreach (var id in parent.childIdList)
        {
            yield return GetRoomNode(id);
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