using System.Collections;
using UnityEngine;

public static class HelperUtilities
{
    public static Camera mainCamera;

    public static bool ValidateCheckEmptyString(Object thisObject, string fileName, string stringToCheck)
    {
        if (stringToCheck == "")
        {
            Debug.Log(fileName + " is empty and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckNullValue(Object thisObject, string fileName, UnityEngine.Object objectToCheck)
    {
        if (objectToCheck == null)
        {
            Debug.Log(fileName + " is null and must contain a value in object " + thisObject.name.ToString());
            return true;
        }
        return false;
    }

    public static bool ValidateCheckEnumerableValues(Object thisObject, string fileName, IEnumerable enumerableObjectToCheck)
    {
        bool error = false;
        int count = 0;

        if (enumerableObjectToCheck == null)
        {
            Debug.Log(fileName + " is null in object " + thisObject.name.ToString());
            return true;
        }

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

    public static bool ValidateCheckPositiveValue(Object thisObject, string fileName, int valueToCheck, bool isZeroAllowed)
    {
        bool error = false;

        if (isZeroAllowed)
        {
            if (valueToCheck < 0)
            {
                Debug.Log(fileName + " must contain a positive value or zero in object " + thisObject.name.ToString());
                error = true;
            }
        }
        else
        {
            if (valueToCheck <= 0)
            {
                Debug.Log(fileName + " must contain a positive value in object " + thisObject.name.ToString());
                error = true;
            }
        }

        return error;
    }

    internal static Vector3 GetNearestSpawnPoint(Vector3 position)
    {
        var currentRoom = GameManager.Instance.CurrentRoom;
        var grid = currentRoom.instantiatedRoom.grid;

        var nearestSpawnPosition = new Vector3(10000f, 10000f, 0f);

        foreach (var spawnPosition in currentRoom.spawnPositions)
        {
            var spawnPositionWorld = grid.CellToWorld((Vector3Int)spawnPosition);

            if (Vector3.Distance(spawnPositionWorld, position) < Vector3.Distance(nearestSpawnPosition, position))
            {
                nearestSpawnPosition = spawnPositionWorld;
            }
        }

        return nearestSpawnPosition;
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

    public static Vector3 GetWorldMousePosition()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseScreenPosition.x = Mathf.Clamp(mouseScreenPosition.x, 0f, Screen.width);
        mouseScreenPosition.y = Mathf.Clamp(mouseScreenPosition.y, 0f, Screen.height);

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition.z = 0f;

        return mouseScreenPosition;
    }

    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x);
        float degree = radians * Mathf.Rad2Deg;
        return degree;
    }

    public static AimDirection GetAimDirection(float angle)
    {
        AimDirection direction;

        if (angle > 22f && angle <= 67f)
        {
            direction = AimDirection.UpRight;
        }
        else if (angle > 67f && angle <= 112f)
        {
            direction = AimDirection.Up;
        }
        else if (angle > 112f && angle <= 158f)
        {
            direction = AimDirection.UpLeft;
        }
        else if ((angle > 159f && angle <= 180f) || (angle > -180f && angle <= -135f))
        {
            direction = AimDirection.Left;
        }
        else if (angle > -135f && angle <= -45f)
        {
            direction = AimDirection.Down;
        }
        else if ((angle > -45f && angle <= 0f) || (angle > 0f && angle <= 22f))
        {
            direction = AimDirection.Right;
        }
        else
        {
            direction = AimDirection.Right;
        }

        return direction;
    }
}
