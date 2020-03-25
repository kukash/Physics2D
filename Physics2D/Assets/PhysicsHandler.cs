using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsHandler : MonoBehaviour
{

    private List<PhysicsComponent> _physicsComponents;
    private List<SpherePhysicsComponent> _sphereColliders;
    private List<GameObject> _PhysicsGameObjects;

    private List<GameObject> _CollidedObjects;
    void Start()
    {
        LoadPhysicsObjects();
    }

    private void LoadPhysicsObjects()
    {
        //init lists
        _CollidedObjects = new List<GameObject>();
        _physicsComponents = new List<PhysicsComponent>();
        _sphereColliders = new List<SpherePhysicsComponent>();

        //get all tagged gameObjects
        GameObject[] newObjects = GameObject.FindGameObjectsWithTag("PhysicsObject");
        _PhysicsGameObjects = newObjects.ToList();
        Debug.Log("Registered " + _PhysicsGameObjects.Count + " new Physics GameObjects");

        //store components in corresponding lists
        foreach (GameObject otherGameObject in _PhysicsGameObjects)
        {
            if (otherGameObject.GetComponent<PhysicsComponent>())
            {
                PhysicsComponent newComponent = otherGameObject.GetComponent<PhysicsComponent>();
                _physicsComponents.Add(newComponent);
                switch (newComponent)
                {
                    case SpherePhysicsComponent c:
                        _sphereColliders.Add(c);
                        break;
                }
            }
            else
            {
                Debug.LogWarning("GameObject Tagged as physics object is missing physics component");
            }
        }
        Debug.Log("Registered " + _sphereColliders.Count + " sphere collider");
    }

    void FixedUpdate()
    {
        StartUpdate();
        DetectCollision();
        ResolveCollision();
    }

    private void StartUpdate()
    {
        foreach (PhysicsComponent component in _physicsComponents)
        {
            component.UpdateComponent();
        }
    }

    private void ResolveCollision()
    {
        DebugCollision();
    }
    private void DetectCollision()
    {
        _CollidedObjects.Clear();
        SphereSphereCollision();
    }

    private void DebugCollision()
    {
        foreach (var VARIABLE in _PhysicsGameObjects)
        {
            if (_CollidedObjects.Contains(VARIABLE))
            {
                VARIABLE.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                VARIABLE.GetComponent<SpriteRenderer>().color = Color.green;

            }
        }
    }

    private void SphereSphereCollision()
    {
        //iterate over sphere colliders
        foreach (SpherePhysicsComponent thisSphere in _sphereColliders)
        {
            foreach (SpherePhysicsComponent otherSphere in _sphereColliders)
            {
                //continue if the spheres comparing are the same
                if (thisSphere == otherSphere) continue;
                //calculate collision and store if spheres collided
                CollisionInfo info = Sphere_SphereIntersection(thisSphere, otherSphere);
                if (info.hit)
                {
                    _CollidedObjects.Add(thisSphere.gameObject);
                    _CollidedObjects.Add(otherSphere.gameObject);
                }
            }
        }
    }

    private CollisionInfo Sphere_SphereIntersection(SpherePhysicsComponent sphereA, SpherePhysicsComponent sphereB)
    {
        CollisionInfo Info = new CollisionInfo();

        float addedRadius = sphereA.GetRadius() + sphereB.GetRadius();
        float distance = Vector2.Distance(sphereA.GetPosition(), sphereB.GetPosition());
        if (addedRadius > distance)
        {
            Info.hit = true;
        }

        return Info;
    }
}


public class CollisionInfo
{
    public bool hit;

    public CollisionInfo()
    {
        hit = false;
    }
}