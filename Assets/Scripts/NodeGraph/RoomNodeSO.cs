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
    public bool isSelected;

    public void Draw(GUIStyle style)
    {
        GUILayout.BeginArea(rect, style);
        EditorGUI.BeginChangeCheck();

        if ((parentIdList.Count > 0 && !type.isNone) || type.isEntrance)
        {
            var textStyle = new GUIStyle();
            textStyle.normal.textColor = Color.black;
            EditorGUILayout.LabelField(type.typeName, textStyle);
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
        switch (MouseEventHelper.GetEvent(currentEvent))
        {
            case MouseEvent.LeftClickDown:
                ProcessLeftClickDownEvent(currentEvent);
                break;
            case MouseEvent.RightClickDown:
                ProcessRightClickDownEvent(currentEvent);
                break;
            case MouseEvent.LeftClickUp:
                ProcessLeftClickUpEvent(currentEvent);
                break;
            case MouseEvent.LeftClickDrag:
                ProcessLeftClickDragEvent(currentEvent);
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

        if (type.isNone)
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

        if (graph.GetRoomNode(id).type.isBossRoom && graph.GetRoomNode(id).parentIdList.Count > 0)
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

    public bool RemoveParentRoomNode(string id)
    {
        if (parentIdList.Contains(id))
        {
            parentIdList.Remove(id);
            return true;
        }
        return false;
    }

    public bool RemoveChildRoomNode(string id)
    {
        if (childIdList.Contains(id))
        {
            childIdList.Remove(id);
            return true;
        }
        return false;
    }

#endif
}