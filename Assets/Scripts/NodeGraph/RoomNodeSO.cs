using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomNodeSO : ScriptableObject
{
    public string id;
    public List<string> parentRoomNodeIdList = new List<string>();
    public List<string> childRoomNodeIdList = new List<string>();
    [HideInInspector] public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    [HideInInspector] public RoomNodeTypeListSO roomNodeTypeList;

#if UNITY_EDITOR
    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging;
    [HideInInspector] public bool isSelected;

    public void Draw(GUIStyle style)
    {
        GUILayout.BeginArea(rect, style);
        EditorGUI.BeginChangeCheck();

        if (parentRoomNodeIdList.Count > 0 || roomNodeType.isEntrance)
        {
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypeToDisplay());

            roomNodeType = roomNodeTypeList.list[selection];
        }

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        GUILayout.EndArea();
    }

    private string[] GetRoomNodeTypeToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];

        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if (roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }

        return roomArray;
    }

    public void Init(Rect rect, RoomNodeGraphSO nodeGraph, RoomNodeTypeSO roomNodeType)
    {
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.rect = rect;
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
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
        roomNodeGraph.roomNodeToDrawLineFrom = this;
        roomNodeGraph.linePosition = currentEvent.mousePosition;
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
        if (id == this.id)
        {
            return false;
        }

        if (childRoomNodeIdList.Contains(id))
        {
            return false;
        }

        childRoomNodeIdList.Add(id);
        return true;
    }

    public bool AddParentRoomNode(string id)
    {
        if (id == this.id)
        {
            return false;
        }

        if (parentRoomNodeIdList.Contains(id))
        {
            return false;
        }

        parentRoomNodeIdList.Add(id);
        return true;
    }

#endif
}