using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Line(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }

    public Vector3 start;
    public Vector3 end;

    public Vector3 direction()
    {
        return end - start;
    }

    public float distance()
    {
        return Vector3.Distance(start, end);
    }

    static public bool IntersectAny(Line line1, List<Line> lines)
    {
        foreach (var line2 in lines)
        {
            if (Line.Intersect(line1, line2))
            {
                return true;
            }
        }

        return false;
    }

    static public bool Intersect(Line line1, Line line2)
    {
        var A = line1.start;
        var B = line1.end;
        var C = line2.start;
        var D = line2.end;

        return counterClockwise(A, C, D) != counterClockwise(B, C, D) && counterClockwise(A, B, C) != counterClockwise(A, B, D);
    }

    static private bool counterClockwise(Vector3 A, Vector3 B, Vector3 C)
    {
        return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
    }
}

