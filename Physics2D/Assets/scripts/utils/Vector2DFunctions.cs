using System.Collections;
using System.Collections.Generic;
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
}
