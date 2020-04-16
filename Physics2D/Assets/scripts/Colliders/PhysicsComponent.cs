using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PhysicsComponent : MonoBehaviour
{
    public PhysicsInfo Info;
    void Start()
    {

    }
    public virtual void InitPhysicsComponent() { }

    public virtual bool IsStatic()
    {
        return false;
    }
    public Vector2 GetPosition()
    {
        return Info.NewPosition;
    }

    public Vector2 GetOldPosition()
    {
        return Info.OldPosition;
    }
    public virtual void Step() { }

    public void UpdateComponent()
    {
        Vector2DFunctions.Update2DTransform(Info.NewPosition, this);
    }

}
