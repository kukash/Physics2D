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
    private Dictionary<GameObject, PhysicsComponent> _gameObjectPhysicsComponentMap;

    private List<GameObject> _CollidedObjects;
    private Dictionary<GameObject, CollisionInfo> _CollidedObjectsMap;
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
        _gameObjectPhysicsComponentMap = new Dictionary<GameObject, PhysicsComponent>();
        _CollidedObjectsMap = new Dictionary<GameObject, CollisionInfo>();
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
                _gameObjectPhysicsComponentMap.Add(otherGameObject, newComponent);
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

        UpdateObjects();
    }

    private void UpdateObjects()
    {
        foreach (PhysicsComponent component in _physicsComponents)
        {
            if (!component.IsStatic()) component.UpdateComponent();
        }
    }

    private void StartUpdate()
    {
        foreach (PhysicsComponent component in _physicsComponents)
        {
            //isStatic always returns false if it is not overwritten
            if (!component.IsStatic()) component.Step();
        }
    }

    private void ResolveCollision()
    {
        DebugCollision();
    }
    private void DetectCollision()
    {
        _CollidedObjects.Clear();
        _CollidedObjectsMap.Clear();
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
                _gameObjectPhysicsComponentMap[physicsGameObject].Info.Speed = 0;
                physicsGameObject.GetComponent<PhysicsInfo>().NewPosition =
                    _CollidedObjectsMap[physicsGameObject].PointOfImpact;
            }
            else
            {
                if (!physicsGameObject.GetComponent<PhysicsComponent>().IsStatic())
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
                    Vector2 poi = SphereLinePOI(sphere, line);
                    info.PointOfImpact = poi;
                    _CollidedObjectsMap.Add(sphere.gameObject, info);
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

    private Vector2 SphereLinePOI(SpherePhysicsComponent sphere, LineCollider line)
    {

        PhysicsInfo sphereInfo = sphere.GetComponent<PhysicsInfo>();
        Vector2 pointOfImpact = Vector2.zero;

        if (sphereInfo.Speed > 0)
        {
            //Debug.Log("Old position" + sphereInfo.OldPosition);
            //Debug.Log("old distance :" + SphereLineDistance(sphereInfo.OldPosition, line));
            //Debug.Log("New position" + sphereInfo.NewPosition);
            //Debug.Log("new distance :" + SphereLineDistance(sphereInfo.NewPosition, line));
            float oldDistance = SphereLineDistance(sphereInfo.OldPosition, line);
            float newDistance = SphereLineDistance(sphereInfo.NewPosition, line);
            float a = oldDistance - sphere.GetRadius();
            float b = oldDistance + newDistance;
            //   Vector2 delta = sphereInfo.NewPosition - sphereInfo.OldPosition;

            // float b = Vector2DFunctions.Length(delta);
            float t = a / b;

            Vector2 offset = sphereInfo.Direction.normalized * t * sphereInfo.Velocity;
            //a<b
            //Debug.Log("Delta " + delta);
            //Debug.Log("a: " + a);
            //Debug.Log("b: " + b);
            //Debug.Log("t: " + t);
            //Debug.Log("speed " + sphereInfo.Speed);
            //Debug.Log("Dir " + sphereInfo.Direction);
            //Debug.Log("offset " + offset);
            pointOfImpact = sphereInfo.OldPosition + offset;
            // Debug.Log("poi: " + POI);

        }

        return pointOfImpact;
    }

    private float SphereLineDistance(Vector2 position, LineCollider line)
    {
        float distance = 0;
        Vector2 Delta = position - line.GetLineStart();

        float dot = Vector2.Dot(line.GetLineDir(), Delta);
        Vector2 newPos = line.GetLineStart() + line.GetLineDir().normalized * dot;

        distance = Vector2.Distance(newPos, position);
        return distance;

    }
    private CollisionInfo Sphere_Line_Intersection(SpherePhysicsComponent sphere, LineCollider line)
    {
        CollisionInfo info = new CollisionInfo();

        if (SphereLineDistance(sphere.GetPosition(), line) < sphere.GetRadius()) info.hit = true;
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
    public Vector2 PointOfImpact = Vector2.zero;
    public CollisionInfo()
    {
        hit = false;
    }
}