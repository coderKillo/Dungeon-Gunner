using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperUtilities
{
    public static bool ValidateCheckEmptyString(Object thisObject, string fileName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fileName + " is empty and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fileName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        foreach (var item in enumerableObjectToCheck)
        {
            if (item == null)
            {
                Debug.Log(fileName + " has null value in object " + thisObject.name.ToString());
                error = true;
            }
            else
            {
                count++;
            }

        }

        if (count == 0)
        {
            Debug.Log(fileName + " has no values in object " + thisObject.name.ToString());
            error = true;
        }

        return error;
    }

    public static Vector2 RoundDirectionTo90Degree(Vector2 point1, Vector2 point2)
    {
        var direction = (point2 - point1).normalized;

        if (direction.x > 0.7)
        {
            direction = Vector2.right;
        }
        else if (direction.x < -0.7)
        {
            direction = Vector2.left;
        }
        else if (direction.y > 0.7)
        {
            direction = Vector2.up;
        }
        else if (direction.y < -0.7)
        {
            direction = Vector2.down;
        }
        return direction;
    }
}
