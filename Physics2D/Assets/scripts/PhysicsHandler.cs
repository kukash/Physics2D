using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhysicsHandler : MonoBehaviour
{

    private List<PhysicsComponent> _physicsComponents;
    private List<SpherePhysicsComponent> _sphereColliders;
    private List<LineCollider> _lineColliders;
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
        _lineColliders = new List<LineCollider>();

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
                    case LineCollider c:
                        _lineColliders.Add(c);
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
            //isStatic always returns false if it is not overwritten
            if (!component.IsStatic()) component.UpdateComponent();
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
        SphereLineCollision();
    }

    private void DebugCollision()
    {
        foreach (GameObject physicsGameObject in _PhysicsGameObjects)
        {
            if (_CollidedObjects.Contains(physicsGameObject))
            {
                physicsGameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
            else
            {
                if(!physicsGameObject.GetComponent<PhysicsComponent>().IsStatic())
                    physicsGameObject.GetComponent<SpriteRenderer>().color = Color.green;

            }
        }
    }

    private void SphereLineCollision()
    {
        foreach (SpherePhysicsComponent sphere in _sphereColliders)
        {
            foreach (LineCollider line in _lineColliders)
            {
                CollisionInfo info = Sphere_Line_Intersection(sphere, line);
                if (info.hit)
                {
                    _CollidedObjects.Add(sphere.gameObject);
                }
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

    private CollisionInfo Sphere_Line_Intersection(SpherePhysicsComponent sphere, LineCollider line)
    {
        CollisionInfo info = new CollisionInfo();
        Vector2 Delta = sphere.GetPosition() -line.GetLineStart() ;
      
        float dot = Vector2.Dot(line.GetLineDir(), Delta);
        Vector2 newPos = line.GetLineStart() + line.GetLineDir().normalized * dot;

        float distance = Vector2.Distance(newPos, sphere.GetPosition());
        if (distance < sphere.GetRadius()) info.hit = true;
        return info;
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