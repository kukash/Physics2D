using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpherePhysicsComponent : PhysicsComponent
{
    private float Radius;
    private sphereMovement sphereController;
    public float GetRadius()
    {
        return Radius;
    }
    public override void InitPhysicsComponent()
    {
        Radius = transform.localScale.x*0.5f;
        sphereController = GetComponent<sphereMovement>();
    }

    protected override void OtherUpdates()
    {
        sphereController?.Move();
    }
}
