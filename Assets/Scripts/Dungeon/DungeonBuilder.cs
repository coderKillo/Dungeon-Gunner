using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[DisallowMultipleComponent]
public class DungeonBuilder : SingletonAbstract<DungeonBuilder>
{
    #region HEADER
    public Dictionary<string, Room> roomDictionary = new Dictionary<string, Room>();

    private Dictionary<string, RoomTemplateSO> templateDictionary = new Dictionary<string, RoomTemplateSO>();
    private List<RoomTemplateSO> templateList = new List<RoomTemplateSO>();
    private RoomNodeTypeListSO typeList;
    private bool buildSuccessful;
    #endregion

    protected override void Awake()
    {
        base.Awake();

        typeList = GameResources.Instance.roomNodeTypeList;

        GameResources.Instance.dimmedMaterial.SetFloat("Alpha_Slider", 1f);
    }

    public RoomTemplateSO GetRoomTemplate(string id)
    {
        if (templateDictionary.TryGetValue(id, out RoomTemplateSO template))
        {
            return template;
        }

        return null;
    }

    public Room GetRoom(string id)
    {
        if (roomDictionary.TryGetValue(id, out Room room))
        {
            return room;
        }

        return null;
    }

    public bool GenerateDungeon(DungeonLevelSO dungeonLevel)
    {
        templateList = dungeonLevel.roomTemplateList;

        LoadRoomTemplateIntoDictionary();

        buildSuccessful = false;
        int buildAttempts = 0;

        while (!buildSuccessful && buildAttempts < Settings.maxDungeonBuildAttempts)
        {
            buildAttempts++;

            var graph = dungeonLevel.roomNodeGraphList[UnityEngine.Random.Range(0, dungeonLevel.roomNodeGraphList.Count)];

            buildSuccessful = false;
            int rebuildAttemptsForGraph = 0;

            while (!buildSuccessful && rebuildAttemptsForGraph < Settings.maxDungeonRebuildAttemptsForRoomGraph)
            {
                rebuildAttemptsForGraph++;

                ClearDungeon();

                buildSuccessful = BuildRandomDungeon(graph);

            }

            if (buildSuccessful)
            {
                InstantiateRoomGameObjects();
            }
        }

        return buildSuccessful;
    }

    private void InstantiateRoomGameObjects()
    {
        foreach (var room in roomDictionary.Values)
        {
            var roomPosition = new Vector3(room.lowerBound.x - room.templateLowerBound.x, room.lowerBound.y - room.templateLowerBound.y);
            var roomGameObject = GameObject.Instantiate(room.prefab, roomPosition, Quaternion.identity, transform);

            var instantiatedRoom = roomGameObject.GetComponentInChildren<InstantiatedRoom>();
            instantiatedRoom.room = room;
            instantiatedRoom.Initialize(roomGameObject);

            room.instantiatedRoom = instantiatedRoom;
        }
    }

    private bool BuildRandomDungeon(RoomNodeGraphSO graph)
    {
        var roomNodeQueue = new Queue<RoomNodeSO>();

        var entranceNode = graph.GetRoomNode(typeList.list.Find(x => x.isEntrance));
        if (entranceNode == null)
        {
            Debug.Log("No Entrance Node");
            return false;
        }

        roomNodeQueue.Enqueue(entranceNode);

        bool roomOverlap = false;
        roomOverlap = ProcessRoomNodeQueue(graph, roomNodeQueue, roomOverlap);

        if (roomNodeQueue.Count > 0 || roomOverlap)
        {
            return false;
        }

        return true;
    }

    private bool ProcessRoomNodeQueue(RoomNodeGraphSO graph, Queue<RoomNodeSO> roomNodeQueue, bool roomOverlap)
    {
        while (roomNodeQueue.Count > 0 && !roomOverlap)
        {
            var roomNode = roomNodeQueue.Dequeue();

            foreach (var child in graph.GetChildRoomNode(roomNode))
            {
                roomNodeQueue.Enqueue(child);
            }

            if (roomNode.type.isEntrance)
            {
                var roomTemplate = GetRandomTemplate(roomNode.type);

                var room = CreateRoomFromTemplate(roomTemplate, roomNode);
                room.isPositioned = true;

                roomDictionary.Add(room.id, room);
            }
            else
            {
                var parentRoom = roomDictionary[roomNode.parentIdList[0]];

                roomOverlap = !CanPlaceRoom(roomNode, parentRoom);
            }
        }

        return roomOverlap;
    }

    private bool CanPlaceRoom(RoomNodeSO roomNode, Room parentRoom)
    {
        bool roomOverlaps = true;

        while (roomOverlaps)
        {
            var unconnectedParentDoorways = parentRoom.doorwayList.FindAll(x => (!x.isConnected && !x.isUnavailable));

            if (unconnectedParentDoorways.Count == 0)
            {
                return false;
            }

            var parentDoorway = unconnectedParentDoorways[UnityEngine.Random.Range(0, unconnectedParentDoorways.Count)];

            var roomTemplate = GetRandomTemplateConsistentWithParent(roomNode, parentDoorway);

            var room = CreateRoomFromTemplate(roomTemplate, roomNode);

            if (PlaceTheRoom(parentRoom, parentDoorway, room))
            {
                roomOverlaps = false;

                room.isPositioned = true;

                roomDictionary.Add(room.id, room);
            }
        }

        return true;
    }

    private bool PlaceTheRoom(Room parentRoom, Doorway parentDoorway, Room room)
    {
        var doorway = GetOppositeDoorway(parentDoorway, room.doorwayList);

        if (doorway == null)
        {
            parentDoorway.isUnavailable = true;
            return false;
        }

        var parentDoorwayPosition = parentRoom.lowerBound + parentDoorway.position - parentRoom.templateLowerBound;
        var adjustment = Vector2Int.zero;

        switch (doorway.orientation)
        {
            case Orientation.north:
                adjustment = new Vector2Int(0, -1);
                break;
            case Orientation.south:
                adjustment = new Vector2Int(0, 1);
                break;
            case Orientation.west:
                adjustment = new Vector2Int(1, 0);
                break;
            case Orientation.east:
                adjustment = new Vector2Int(-1, 0);
                break;
            default:
                break;
        }

        room.lowerBound = parentDoorwayPosition + adjustment + room.templateLowerBound - doorway.position;
        room.upperBound = room.lowerBound + room.templateUpperBound - room.templateLowerBound;

        if (CheckForRoomOverlap(room) == null)
        {
            parentDoorway.isConnected = true;
            parentDoorway.isUnavailable = true;

            doorway.isConnected = true;
            doorway.isUnavailable = true;

            return true;
        }
        else
        {
            parentDoorway.isUnavailable = true;
            return false;
        }

    }

    private Room CheckForRoomOverlap(Room roomToTest)
    {
        var roomToTestRect = new Rect(roomToTest.lowerBound, roomToTest.upperBound - roomToTest.upperBound);

        foreach (var room in roomDictionary.Values)
        {
            if (room.id == roomToTest.id || !room.isPositioned)
            {
                continue;
            }

            var roomRect = new Rect(room.lowerBound, room.upperBound - room.upperBound);

            if (roomToTestRect.Overlaps(roomRect))
            {
                return room;
            }
        }

        return null;
    }

    private Doorway GetOppositeDoorway(Doorway parentDoorway, List<Doorway> doorwayList)
    {
        foreach (var doorway in doorwayList)
        {
            if (parentDoorway.orientation == Orientation.north && doorway.orientation == Orientation.south)
            {
                return doorway;
            }
            else if (parentDoorway.orientation == Orientation.south && doorway.orientation == Orientation.north)
            {
                return doorway;
            }
            else if (parentDoorway.orientation == Orientation.east && doorway.orientation == Orientation.west)
            {
                return doorway;
            }
            else if (parentDoorway.orientation == Orientation.west && doorway.orientation == Orientation.east)
            {
                return doorway;
            }

        }

        return null;
    }

    private RoomTemplateSO GetRandomTemplateConsistentWithParent(RoomNodeSO roomNode, Doorway parentDoorway)
    {
        RoomTemplateSO template = null;

        if (roomNode.type.isCorridor)
        {
            switch (parentDoorway.orientation)
            {
                case Orientation.north:
                case Orientation.south:
                    template = GetRandomTemplate(typeList.list.Find(x => x.isCorridorNS));
                    break;

                case Orientation.east:
                case Orientation.west:
                    template = GetRandomTemplate(typeList.list.Find(x => x.isCorridorEW));
                    break;

                case Orientation.none:
                default:
                    break;
            }

        }
        else
        {
            template = GetRandomTemplate(roomNode.type);
        }

        return template;
    }

    private Room CreateRoomFromTemplate(RoomTemplateSO roomTemplate, RoomNodeSO roomNode)
    {
        var room = new Room();

        room.id = roomNode.id;
        room.childIdList = CopyStringList(roomNode.childIdList);

        room.templateId = roomTemplate.guid;
        room.prefab = roomTemplate.prefab;
        room.nodeType = roomTemplate.roomNodeType;
        room.lowerBound = roomTemplate.lowerBounds;
        room.upperBound = roomTemplate.upperBounds;
        room.templateLowerBound = roomTemplate.lowerBounds;
        room.templateUpperBound = roomTemplate.upperBounds;
        room.spawnPositions = roomTemplate.spawnPositionArray;
        room.doorwayList = CopyDoorwayList(roomTemplate.doorwayList);

        if (roomNode.parentIdList.Count == 0)
        {
            room.parentId = "";
            room.isPreviouslyVisited = true;
        }
        else
        {
            room.parentId = roomNode.parentIdList[0];
        }

        return room;
    }

    private RoomTemplateSO GetRandomTemplate(RoomNodeTypeSO type)
    {
        var matchingTemplate = templateList.FindAll(x => x.roomNodeType == type);
        if (matchingTemplate.Count > 0)
        {
            return matchingTemplate[UnityEngine.Random.Range(0, matchingTemplate.Count)];
        }

        return null;
    }

    private void ClearDungeon()
    {
        if (roomDictionary.Count <= 0)
        {
            return;
        }

        foreach (var room in roomDictionary.Values)
        {
            if (room.instantiatedRoom != null)
            {
                Destroy(room.instantiatedRoom.gameObject);
            }
        }

        roomDictionary.Clear();
    }

    private void LoadRoomTemplateIntoDictionary()
    {
        templateDictionary.Clear();

        foreach (var template in templateList)
        {
            if (!templateDictionary.ContainsKey(template.guid))
            {
                templateDictionary.Add(template.guid, template);
            }
            else
            {
                Debug.Log("Duplicate Room Template Key in " + templateList);
            }
        }
    }

    private List<Doorway> CopyDoorwayList(List<Doorway> oldDoorwayList)
    {
        List<Doorway> newDoorwayList = new List<Doorway>();

        foreach (Doorway doorway in oldDoorwayList)
        {
            Doorway newDoorway = new Doorway();

            newDoorway.position = doorway.position;
            newDoorway.orientation = doorway.orientation;
            newDoorway.doorPrefab = doorway.doorPrefab;
            newDoorway.isConnected = doorway.isConnected;
            newDoorway.isUnavailable = doorway.isUnavailable;
            newDoorway.doorwayStartCopyPosition = doorway.doorwayStartCopyPosition;
            newDoorway.doorwayCopyTileWidth = doorway.doorwayCopyTileWidth;
            newDoorway.doorwayCopyTileHeight = doorway.doorwayCopyTileHeight;

            newDoorwayList.Add(newDoorway);
        }

        return newDoorwayList;
    }


    /// <summary>
    /// Create deep copy of string list
    /// </summary>
    private List<string> CopyStringList(List<string> oldStringList)
    {
        List<string> newStringList = new List<string>();

        foreach (string stringValue in oldStringList)
        {
            newStringList.Add(stringValue);
        }

        return newStringList;
    }

}
