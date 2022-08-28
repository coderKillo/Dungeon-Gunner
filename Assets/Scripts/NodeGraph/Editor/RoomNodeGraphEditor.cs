using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;

public class RoomNodeGraphEditor : EditorWindow
{
    private static RoomNodeGraphSO currentGraph;
    private RoomNodeSO currentRoomNode = null;
    private RoomNodeTypeListSO typeList;

    private GUIStyle defaultStyle;

    private int nodePadding = 25;
    private int nodeBorder = 12;
    private float nodeWidth = 160f;
    private float nodeHeight = 75f;

    private float connectLineWith = 3f;
    private float connectLineArrowSize = 6f;

    private void OnEnable()
    {
        Selection.selectionChanged += InspectorSelectionChanged;

        defaultStyle = new GUIStyle();
        defaultStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        defaultStyle.normal.textColor = Color.white;
        defaultStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        defaultStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        typeList = GameResources.Instance.roomNodeTypeList;
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    private void InspectorSelectionChanged()
    {
        RoomNodeGraphSO roomNodeGraph = Selection.activeObject as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            currentGraph = roomNodeGraph;
            GUI.changed = true;
        }
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

            currentGraph = roomNodeGraph;

            return true;
        }

        return false;
    }

    private void OnGUI()
    {
        if (currentGraph != null)
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
        foreach (var roomNode in currentGraph.nodeList)
        {
            foreach (var childRoomNodeId in roomNode.childIdList)
            {
                if (!currentGraph.nodeDictionary.ContainsKey(childRoomNodeId))
                {
                    continue;
                }

                DrawConnectionLine(roomNode, currentGraph.nodeDictionary[childRoomNodeId]);

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
        if (currentGraph.linePosition != Vector2.zero)
        {
            Handles.DrawBezier(
                currentGraph.roomNodeToDrawLineFrom.rect.center,
                currentGraph.linePosition,
                currentGraph.roomNodeToDrawLineFrom.rect.center,
                currentGraph.linePosition,
                Color.white,
                null,
                connectLineWith
            );
        }
    }

    private void DrawRoomNodes()
    {
        foreach (RoomNodeSO node in currentGraph.nodeList)
        {
            var style = new GUIStyle(defaultStyle);

            string textureName = node.type.GetStyleTextureName();
            if (node.isSelected)
            {
                textureName += " on";
            }

            style.normal.background = EditorGUIUtility.Load(textureName) as Texture2D;

            node.Draw(style);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        if (currentRoomNode == null || currentRoomNode.isLeftClickDragging == false)
        {
            currentRoomNode = isMouseOverRoomNode(currentEvent);
        }

        if (currentRoomNode == null || currentGraph.roomNodeToDrawLineFrom != null)
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
        foreach (var roomNode in currentGraph.nodeList)
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
                else if (currentEvent.button == 0)
                {
                    ClearLineDrag();
                    ClearSelectedRoomNodes();
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

    private void ClearSelectedRoomNodes()
    {
        foreach (var roomNode in currentGraph.nodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    private void ProcessRightMouseUpEvent(Event currentEvent)
    {
        if (currentGraph.roomNodeToDrawLineFrom != null)
        {
            var roomNode = isMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                if (currentGraph.roomNodeToDrawLineFrom.AddChildRoomNode(roomNode.id))
                {
                    roomNode.AddParentRoomNode(currentGraph.roomNodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }
    }

    private void ClearLineDrag()
    {
        currentGraph.roomNodeToDrawLineFrom = null;
        currentGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentGraph.roomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    private void DragConnectingLine(Vector2 delta)
    {
        currentGraph.linePosition += delta;
    }

    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();

        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePosition)
    {
        if (currentGraph.nodeList.Count == 0)
        {
            CreateRoomNode(new Vector2(200, 200), typeList.list.Find(x => x.isEntrance));
        }
        CreateRoomNode(mousePosition, typeList.list.Find(x => x.isNone));
    }

    private void CreateRoomNode(object mousePositionObject, RoomNodeTypeSO roomNodeType)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;
        RoomNodeSO roomNode = ScriptableObject.CreateInstance<RoomNodeSO>();

        currentGraph.nodeList.Add(roomNode);

        roomNode.Init(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentGraph, roomNodeType);

        AssetDatabase.AddObjectToAsset(roomNode, currentGraph);
        AssetDatabase.SaveAssets();

        currentGraph.LoadListToDictionary();
    }
}
