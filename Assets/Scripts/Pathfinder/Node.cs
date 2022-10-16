

using System.Numerics;
using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector2Int position;
    public Node parent;

    public int hCost = 0;
    public int gCost = 0;
    public int FCost { get { return hCost + gCost; } }

    public Node(Vector2Int position)
    {
        this.position = position;
        this.parent = null;
    }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);

        if (compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }

        return compare;
    }
}
