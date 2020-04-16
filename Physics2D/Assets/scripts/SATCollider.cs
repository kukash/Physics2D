using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SATCollider : PhysicsComponent
{
    public enum SATType
    {
        BOX_COLLIDER
    }

    [SerializeField] private SATType Type = SATType.BOX_COLLIDER;
    public float radiusX;
    public float radiusY;
    public bool Static;
    public Axis[] Axises;
    public override void InitPhysicsComponent()
    {

        Info = GetComponent<PhysicsInfo>();
        radiusX = transform.localScale.x * 0.5f;
        radiusY = transform.localScale.y * 0.5f;
        Info.OldPosition = Vector2DFunctions.GetTransform2D(this);
        Info.NewPosition = Info.OldPosition;
        Axises = new Axis[4];
        Info.verticies = new Vector2[4];
        Info.IsStatic = Static;
        Info.radius = transform.localScale.x / 2;
        WriteAxises();

        if (GetComponent<MoverComponent>())
        {
            Info.mover = GetComponent<MoverComponent>();
            Info.mover.Init();
            Info.Speed = Info.mover.GetSpeed();
        }
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

        Info.verticies[0] = Info.OldPosition - rightVec - upVec;
        Info.verticies[1] = Info.OldPosition + rightVec - upVec;
        Info.verticies[2] = Info.OldPosition + rightVec + upVec;
        Info.verticies[3] = Info.OldPosition - rightVec + upVec;

        Axises[0] = new Axis(Info.verticies[0], Info.verticies[1]);
        Axises[1] = new Axis(Info.verticies[1], Info.verticies[2]);
        Axises[2] = new Axis(Info.verticies[2], Info.verticies[3]);
        Axises[3] = new Axis(Info.verticies[0], Info.verticies[3]);
    }

    public override void Step()
    {

        if (!Static)
        {
            Info.OldPosition = Vector2DFunctions.GetTransform2D(this);
            Vector2DFunctions.Update2DTransform(Info.NewPosition, this);
            WriteAxises();
            //   Debug.Log("not static");
            if (Info.mover)
            {
                Info.mover.Step();
                // Debug.Log("has mover");
            }
        }
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