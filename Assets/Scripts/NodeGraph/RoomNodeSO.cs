using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    public string id;
    public List<string> parentIdList = new List<string>();
    public List<string> childIdList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO graph;
    public RoomNodeTypeSO type;
    [HideInInspector] public RoomNodeTypeListSO typeList;

#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging;
    [HideInInspector] public bool isSelected;

    public void Draw(GUIStyle style)
    {
        GUILayout.BeginArea(rect, style);
        EditorGUI.BeginChangeCheck();

        if (parentIdList.Count > 0 || type.isEntrance)
        {
            EditorGUILayout.LabelField(type.typeName);
        }
        else
        {
            int selected = typeList.list.FindIndex(x => x == type);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypeToDisplay());

            type = typeList.list[selection];
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        GUILayout.EndArea();
    }

    private string[] GetRoomNodeTypeToDisplay()
    {
        string[] roomArray = new string[typeList.list.Count];

        for (int i = 0; i < typeList.list.Count; i++)
        {
            if (typeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = typeList.list[i].typeName;
            }
        }

        return roomArray;
    }

    public void Init(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.rect = rect;
        this.graph = nodeGraph;
        this.type = roomNodeType;

        typeList = GameResources.Instance.roomNodeTypeList;
    }

    public void ProcessEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                if (currentEvent.button == 0)
                {
                    ProcessLeftClickDownEvent(currentEvent);
                }
                if (currentEvent.button == 1)
                {
                    ProcessRightClickDownEvent(currentEvent);
                }
                break;
            case EventType.MouseUp:
                if (currentEvent.button == 0)
                {
                    ProcessLeftClickUpEvent(currentEvent);
                }
                break;
            case EventType.MouseDrag:
                if (currentEvent.button == 0)
                {
                    ProcessLeftClickDragEvent(currentEvent);
                }
                break;

            default:
                break;
        }
    }

    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        graph.roomNodeToDrawLineFrom = this;
        graph.linePosition = currentEvent.mousePosition;
    }

    private void ProcessLeftClickDownEvent(Event currentEvent)
    {
        Selection.activeObject = this;
        isSelected = !isSelected;
    }

    private void ProcessLeftClickUpEvent(Event currentEvent)
    {
        isLeftClickDragging = false;
    }

    private void ProcessLeftClickDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        rect.position += currentEvent.delta;

        EditorUtility.SetDirty(this);
        GUI.changed = true;
    }

    public bool AddChildRoomNode(string id)
    {
        if (IsChildRoomValid(id))
        {
            childIdList.Add(id);
            return true;
        }
        return false;
    }

    private bool IsChildRoomValid(string id)
    {
        if (id == this.id)
        {
            return false;
        }

        if (childIdList.Contains(id))
        {
            return false;
        }

        if (parentIdList.Contains(id))
        {
            return false;
        }

        bool isConnectedBossRoomAlready = false;
        foreach (var roomNode in graph.nodeList)
        {
            if (roomNode.type.isBossRoom && roomNode.parentIdList.Count > 0)
            {
                isConnectedBossRoomAlready = true;
            }
        }

        if (graph.GetRoomNode(id).type.isBossRoom && isConnectedBossRoomAlready)
        {
            return false;
        }

        if (graph.GetRoomNode(id).type.isNone)
        {
            return false;
        }

        if (graph.GetRoomNode(id).parentIdList.Count > 0)
        {
            return false;
        }

        if (graph.GetRoomNode(id).type.isCorridor && type.isCorridor)
        {
            return false;
        }

        if (!graph.GetRoomNode(id).type.isCorridor && !type.isCorridor)
        {
            return false;
        }

        if (graph.GetRoomNode(id).type.isCorridor && childIdList.Count > Settings.maxChildCorridors)
        {
            return false;
        }

        if (graph.GetRoomNode(id).type.isEntrance)
        {
            return false;
        }

        if (!graph.GetRoomNode(id).type.isCorridor && childIdList.Count > 0)
        {
            return false;
        }

        return true;
    }

    public bool AddParentRoomNode(string id)
    {
        if (id == this.id)
        {
            return false;
        }

        if (parentIdList.Contains(id))
        {
            return false;
        }

        parentIdList.Add(id);
        return true;
    }

#endif
}