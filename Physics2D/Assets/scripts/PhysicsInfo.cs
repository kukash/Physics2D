using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsInfo : MonoBehaviour
{
    public float Speed;
    public Vector2 Direction = Vector2.zero;
    public Vector2 Velocity = Vector2.zero;
    public Vector2 OldPosition = Vector2.zero;
    public Vector2 NewPosition = Vector2.zero;
    public Vector2[] verticies;
    public bool HasMoved = true;
    public bool IsStatic = true;
    public float radius = 0;
    public MoverComponent mover = null;

}
