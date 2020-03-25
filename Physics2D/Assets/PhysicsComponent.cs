using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsComponent : MonoBehaviour
{
    protected Vector2 position;

    void Start()
    {
        position = new Vector2(transform.position.x, transform.position.y);
        InitPhysicsComponent();
    }
    public virtual void InitPhysicsComponent() { }
    public Vector2 GetPosition()
    {
        return position;
    }

    public void UpdateComponent()
    {
        position = new Vector2(transform.position.x, transform.position.y);
    }

}
