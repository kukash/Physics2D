using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPhysicsCollider : PhysicsComponent
{
    public float radiusX;
    public float radiusY;
    public Axis[] Axises;
    public override void InitPhysicsComponent()
    {
        radiusX = transform.localScale.x * 0.5f;
        radiusY = transform.localScale.y * 0.5f;
        Info = new PhysicsInfo();
        Info.OldPosition = Vector2DFunctions.GetTransform2D(this);
        Info.NewPosition = Info.OldPosition;
        Debug.Log("position : " + Info.OldPosition);
        Debug.Log("scaleX " + radiusX);
        Debug.Log("scaleY " + radiusY);
        Axises = new Axis[4];
        Info.verticies = new Vector2[4];
        WriteAxises();
    }

    private void WriteAxises()
    {
        float rotationAngle = transform.eulerAngles.z;

        Vector2 upVec = Vector2.down;
        upVec = Vector2DFunctions.RotateVec(upVec, rotationAngle);
        upVec = upVec.normalized * radiusY;

        Vector2 rightVec = Vector2.right;
        rightVec = Vector2DFunctions.RotateVec(rightVec, rotationAngle);
        rightVec = rightVec.normalized * radiusX;

        Info.verticies[0] = Info.NewPosition - rightVec - upVec;
        Info.verticies[1] = Info.NewPosition + rightVec - upVec;
        Info.verticies[2] = Info.NewPosition + rightVec + upVec;
        Info.verticies[3] = Info.NewPosition - rightVec + upVec;
  

        //Info.verticies[0] = new Vector2(Info.NewPosition.x - radiusX, Info.NewPosition.y - radiusY);
        //Info.verticies[1] = new Vector2(Info.NewPosition.x + radiusX, Info.NewPosition.y - radiusY);
        //Info.verticies[2] = new Vector2(Info.NewPosition.x + radiusX, Info.NewPosition.y + radiusY);
        //Info.verticies[3] = new Vector2(Info.NewPosition.x - radiusX, Info.NewPosition.y + radiusY);
        Axises[0] = new Axis(Info.verticies[0], Info.verticies[1]);
        Axises[1] = new Axis(Info.verticies[1], Info.verticies[2]);
        Axises[2] = new Axis(Info.verticies[2], Info.verticies[3]);
        Axises[3] = new Axis(Info.verticies[0], Info.verticies[3]);
    }

    public override void Step()
    {
        Info.OldPosition = Vector2DFunctions.GetTransform2D(this);
        Info.NewPosition = Info.OldPosition;
       // WriteAxises();
    }
}
public class Axis
{

    public Vector2 axisStart;
    public Vector2 axisEnd;
    public Vector2 axisDir;
    public Vector2 AxisNormal;

    public Axis()
    {
        axisStart = Vector2.zero;
        axisDir = Vector2.zero;
        axisEnd = Vector2.zero;
    }

    public Axis(Vector2 newStart, Vector2 newEnd)
    {
        UpdateAxis(newStart, newEnd);
    }
    public void UpdateAxis(Vector2 newStart, Vector2 newEnd)
    {
        axisStart = newStart;
        axisEnd = newEnd;
        axisDir = axisEnd - axisStart;
        AxisNormal = new Vector2(axisDir.normalized.y, -axisDir.normalized.x);
        //   axisDir = axisDir.normalized;
    }



}