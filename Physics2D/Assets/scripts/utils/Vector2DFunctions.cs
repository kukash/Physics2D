using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public struct Vector2DFunctions
{

    public static Vector2 RotateVec(Vector2 vec, float angle)
    {
        float radians = angle * Mathf.Deg2Rad;
        Vector2 newVec = Vector2.zero;
        float newX = vec.x * Mathf.Cos(radians) - vec.y * Mathf.Sin(radians);
        float newY = vec.x * Mathf.Sin(radians) + vec.y * Mathf.Cos(radians);
        newVec = new Vector2(newX, newY);
        return newVec;
    }

    public static void Update2DTransform(Vector2 newPos, MonoBehaviour unityScript)
    {
        float z = unityScript.transform.position.z;
        Vector3 pos3D = new Vector3(newPos.x, newPos.y, z);
        unityScript.transform.position = pos3D;
    }
    public static void Update2DTransform(Vector2 newPos, Transform targetTransform)
    {
        float z = targetTransform.position.z;
        Vector3 pos3D = new Vector3(newPos.x, newPos.y, z);
        targetTransform.transform.position = pos3D;
    }

    public static Vector2 PerpendicularClockwise(Vector2 vector)
    {
        return new Vector2(vector.y, -vector.x);
    }
    public static Vector2 PerpendicularCounterClockwise(Vector2 vector)
    {
        return new Vector2(-vector.y, vector.x);
    }
    public static Vector2 GetTransform2D(MonoBehaviour unityScript)
    {
        Vector2 pos = Vector2.zero;
        pos = new Vector2(unityScript.transform.position.x, unityScript.transform.position.y);
        return pos;
    }

    public static float Length(Vector2 vector)
    {
        float length = 0;
        length = Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
        return length;

    }
}
