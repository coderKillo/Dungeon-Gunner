using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

public class RoomNodeGraphEditor : EditorWindow
{
    private static RoomNodeGraphSO currentRoomNodeGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO roomNodeTypeList;

    private GUIStyle roomNodeStyle;
    private int nodePadding = 25;
    private int nodeBorder = 12;
    private float nodeWidth = 160f;
    private float nodeHeight = 75f;

    private float connectLineWith = 3f;
    private float connectLineArrowSize = 6f;

    private void OnEnable()
    {
        roomNodeStyle = new GUIStyle();
        roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        roomNodeStyle.normal.textColor = Color.white;
        roomNodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        roomNodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }

    [MenuItem("Room Node Graph", menuItem = "Window/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<RoomNodeGraphEditor>("Room Node Graph Editor");
    }

    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceId, int line)
    {
        var roomNodeGraph = EditorUtility.InstanceIDToObject(instanceId) as RoomNodeGraphSO;
        if (roomNodeGraph != null)
        {
            OpenWindow();

            currentRoomNodeGraph = roomNodeGraph;

            return true;
        }

        return false;
    }

    private void OnGUI()
    {
        if (currentRoomNodeGraph != null)
        {
            DrawDraggedLine();

            ProcessEvents(Event.current);

            DrawRoomNodeConnections();

            DrawRoomNodes();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void DrawRoomNodeConnections()
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            foreach (var childRoomNodeId in roomNode.childRoomNodeIdList)
            {
                if (!currentRoomNodeGraph.roomNodeDictionary.ContainsKey(childRoomNodeId))
                {
                    continue;
                }

                DrawConnectionLine(roomNode, currentRoomNodeGraph.roomNodeDictionary[childRoomNodeId]);

                GUI.changed = true;
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO from, RoomNodeSO to)
    {
        Vector2 startPoint = from.rect.center;
        Vector2 endPoint = to.rect.center;

        Vector2 midPoint = (endPoint + startPoint) / 2f;
        Vector2 direction = (endPoint - startPoint).normalized;
        Vector2 perpendicularDirection = new Vector2(-direction.y, direction.x).normalized;

        Vector2 arrowTailPoint1 = midPoint - perpendicularDirection * connectLineArrowSize;
        Vector2 arrowTailPoint2 = midPoint + perpendicularDirection * connectLineArrowSize;
        Vector2 arrowHeadPoint = midPoint + direction.normalized * connectLineArrowSize;

        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectLineWith);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectLineWith);
        Handles.DrawBezier(startPoint, endPoint, startPoint, endPoint, Color.white, null, connectLineWith);
    }

    private void DrawDraggedLine()
    {
        if (currentRoomNodeGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                currentRoomNodeGraph.linePosition,
                currentRoomNodeGraph.roomNodeToDrawLineFrom.rect.center,
                currentRoomNodeGraph.linePosition,
                Color.white,
                null,
                connectLineWith
            );
        }
    }

    private void DrawRoomNodes()
    {
        foreach (RoomNodeSO node in currentRoomNodeGraph.roomNodeList)
        {
            node.Draw(roomNodeStyle);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = isMouseOverRoomNode(currentEvent);
        }

        if (currentRoomNode == null || currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            currentRoomNode.ProcessEvents(currentEvent);
        }
    }

    private RoomNodeSO isMouseOverRoomNode(Event currentEvent)
    {
        foreach (var roomNode in currentRoomNodeGraph.roomNodeList)
        {
            if (roomNode.rect.Contains(currentEvent.mousePosition))
            {
                return roomNode;
            }
        }

        return null;
    }

    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            case EventType.MouseDown:
                if (currentEvent.button == 1)
                {
                    ShowContextMenu(currentEvent.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if (currentEvent.button == 1)
                {
                    ProcessRightMouseDragEvent(currentEvent);
                }
                break;
            case EventType.MouseUp:
                if (currentEvent.button == 1)
                {
                    ProcessRightMouseUpEvent(currentEvent);
                }
                break;
            default:
                break;
        }
    }

    private void ProcessRightMouseUpEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            var roomNode = isMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                if (currentRoomNodeGraph.roomNodeToDrawLineFrom.AddChildRoomNode(roomNode.id))
                {
                    roomNode.AddParentRoomNode(currentRoomNodeGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    private void ClearLineDrag()
    {
        currentRoomNodeGraph.roomNodeToDrawLineFrom = null;
        currentRoomNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentRoomNodeGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void DragConnectingLine(Vector2 delta)
    {
        currentRoomNodeGraph.linePosition += delta;
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePosition)
    {
        CreateRoomNode(mousePosition, roomNodeTypeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        currentRoomNodeGraph.roomNodeList.Add(roomNode);

        roomNode.Init(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentRoomNodeGraph, roomNodeType);

        AssetDatabase.AddObjectToAsset(roomNode, currentRoomNodeGraph);
        AssetDatabase.SaveAssets();

        currentRoomNodeGraph.LoadListToDictionary();
    }
}
