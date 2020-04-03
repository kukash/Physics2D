using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPhysicsCollider : PhysicsComponent
{
    public float radiusX;
    public float radiusY;
    public override void InitPhysicsComponent()
    {
        radiusX = transform.localScale.x*0.5f;
        radiusY = transform.localScale.y*0.5f;
        Info = new PhysicsInfo();
        Info.OldPosition = Vector2DFunctions.GetTransform2D(this);
        Info.NewPosition = Info.OldPosition;
        Debug.Log("position : " + Info.OldPosition);
        Debug.Log("scaleX " + radiusX);
        Debug.Log("scaleY " + radiusY);
    }

    public override void Step()
    {
        Info.OldPosition = Vector2DFunctions.GetTransform2D(this);
        Info.NewPosition = Info.OldPosition;
    }
}
