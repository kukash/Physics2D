using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePhysicsComponent : PhysicsComponent
{
    private float Radius;
    private SphereMovement _mover;
    public float GetRadius()
    {
        return Radius;
    }
    public override void InitPhysicsComponent()
    {
        _mover = GetComponent<SphereMovement>();
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
