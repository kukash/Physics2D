using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePhysicsComponent : PhysicsComponent
{
    private float Radius;
    private MoverComponent _mover;
    public float GetRadius()
    {
        return Radius;
    }
    public override void InitPhysicsComponent()
    {
        Info = GetComponent<PhysicsInfo>();

        _mover = GetComponent<MoverComponent>();
        Radius = transform.localScale.x * 0.5f;
    }

    public override void Step()
    {
        if (_mover)
        {
            _mover.Step();
        }
    }
}
