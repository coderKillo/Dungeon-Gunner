using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static Stack<Vector3> BuildPath(Room room, Vector3Int startPosition, Vector3Int endPosition)
    {
        startPosition -= (Vector3Int)room.templateLowerBound;
        endPosition -= (Vector3Int)room.templateLowerBound;

        var openList = new List<Node>();
        var closeList = new HashSet<Node>();

        int width = room.Size.x;
        int height = room.Size.y;
        var grid = new GridNodes(width, height);

        var startNode = grid.GetNode(startPosition.x, startPosition.y);
        var targetNode = grid.GetNode(endPosition.x, endPosition.y);

        var endNode = FindShortestPath(startNode, targetNode, grid, openList, closeList, room.instantiatedRoom);

        if (endNode == null)
        {
            return null;
        }

        return CreatePathStack(endNode, room);
    }

    private static Stack<Vector3> CreatePathStack(Node endNode, Room room)
    {
        var stack = new Stack<Vector3>();

        Node nextNode = endNode;

        while (nextNode != null)
        {
            Vector3Int cellPosition = new Vector3Int(nextNode.position.x + room.templateLowerBound.x, nextNode.position.y + room.templateLowerBound.y, 0);
            Vector3 worldPosition = room.instantiatedRoom.grid.CellToWorld(cellPosition);
            worldPosition += room.instantiatedRoom.grid.cellSize * 0.5f;
            worldPosition.z = 0f;

            stack.Push(worldPosition);

            nextNode = nextNode.parent;
        }

        return stack;
    }

    private static Node FindShortestPath(Node startNode, Node targetNode, GridNodes grid, List<Node> openList, HashSet<Node> closeList, InstantiatedRoom instantiatedRoom)
    {
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            openList = openList.OrderBy(node => node.FCost).ToList();
            var selectedNode = openList[0];

            if (selectedNode == targetNode)
            {
                return selectedNode;
            }

            openList.Remove(selectedNode);
            closeList.Add(selectedNode);

            List<Node> neighborNodes = GetNeighborNodes(selectedNode, grid, instantiatedRoom.room.lowerBound, instantiatedRoom.room.upperBound);
            foreach (var node in neighborNodes)
            {

                if (closeList.Contains(node))
                {
                    continue;
                }

                int movementPenalty = instantiatedRoom.pathfinderMovementPenaltyMatrix[node.position.x, node.position.y];
                if (movementPenalty <= 0)
                {
                    continue;
                }

                int itemObstacle = instantiatedRoom.pathfinderItemObstaclesMatrix[node.position.x, node.position.y];
                if (itemObstacle <= 0)
                {
                    continue;
                }

                int gCost = selectedNode.gCost + GetDistance(selectedNode, node) + movementPenalty;

                if (!openList.Contains(node) || gCost < selectedNode.gCost)
                {
                    node.gCost = gCost;
                    node.hCost = GetDistance(node, targetNode);
                    node.parent = selectedNode;

                    if (!openList.Contains(node))
                    {
                        openList.Add(node);
                    }
                }
            }
        }

        return null;
    }

    private static int GetDistance(Node node, Node targetNode)
    {
        int nodeDistance = 10;
        int nodeDiagonalDistance = 14;

        int distanceX = Mathf.Abs(node.position.x - targetNode.position.x);
        int distanceY = Mathf.Abs(node.position.y - targetNode.position.y);

        int diagonalSteps = Mathf.Min(distanceX, distanceY);
        int straightSteps = Mathf.Max(distanceX, distanceY) - diagonalSteps;

        return straightSteps * nodeDistance + diagonalSteps * nodeDiagonalDistance;
    }

    private static List<Node> GetNeighborNodes(Node selectedNode, GridNodes grid, Vector2Int lowerBound, Vector2Int upperBound)
    {
        List<Node> neighborNodes = new List<Node>();

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (j == 0 && i == 0)
                {
                    continue;
                }

                int x = selectedNode.position.x + i;
                int y = selectedNode.position.y + j;

                if (x < 0 || y < 0)
                {
                    continue;
                }

                if (x >= grid.Width || y >= grid.Height)
                {
                    continue;
                }

                var node = grid.GetNode(x, y);
                if (node != null)
                {
                    neighborNodes.Add(node);
                }
            }
        }

        return neighborNodes;
    }
}
