using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using Random = UnityEngine.Random;

public class PhysicsHandler : MonoBehaviour
{
    public bool spawnRandomObjects = true;
    public int staticObjectCount = 100;
    public int movingObjectsCount = 0;
    public GameObject staticPrefab;
    public GameObject movingPrefab;
    private List<PhysicsComponent> _physicsComponents;
    private List<SpherePhysicsComponent> _sphereColliders;
    private List<LineCollider> _lineColliders;
    private List<SATCollider> _SATColliders;
    private List<GameObject> _PhysicsGameObjects;
    private Dictionary<GameObject, PhysicsComponent> _gameObjectPhysicsComponentMap;

    private List<SATCollider> _movingColliders;

    private List<GameObject> _CollidedObjects;
    private Dictionary<GameObject, CollisionInfo> _CollidedObjectsMap;

    private PartitioningGrid grid;

    private int _leftBound;
    private int _rightBound;
    private int _topBound;
    private int _botBound;
    void Start()
    {
        if (!movingPrefab.GetComponent<PhysicsComponent>() || !staticPrefab.GetComponent<PhysicsComponent>())
        {
            Debug.LogError("PREFAB ASSIGNED DOES NOT HAVE PHYSICS COMPONENT");
        }
        InitLists();
        InitGrid();
        if (spawnRandomObjects) { Spawn(); }
        //  LoadPhysicsObjects();
        //  InitGrid();
    }

    private void Spawn()
    {
        for (int i = 0; i < staticObjectCount; i++)
        {
            createObject(staticPrefab);
        }

        for (int i = 0; i < movingObjectsCount; i++)
        {
            createObject(movingPrefab);
        }
        grid.InitialPhysicsComponentSort(_SATColliders);
    }
    private void createObject(GameObject prefab)
    {
        float newX = Random.Range((float)_leftBound + 5, (float)_rightBound - 5);
        float newY = Random.Range((float)_botBound + 5, (float)_topBound - 5);

        Vector2 pos = new Vector2(newX, newY);
        Debug.Log(pos);
        GameObject newObject = Instantiate(prefab);
        Vector2DFunctions.Update2DTransform(pos, newObject.transform);
        SATCollider collider = newObject.GetComponent<SATCollider>();
        _SATColliders.Add(collider);
        collider.InitPhysicsComponent();
        _PhysicsGameObjects.Add(newObject);
        _physicsComponents.Add(collider);
        _gameObjectPhysicsComponentMap.Add(newObject, collider);
        if (!collider.Static)
        {
            _movingColliders.Add(collider);
        }
    }
    private void InitGrid()
    {
        grid = GetComponent<PartitioningGrid>();
        if (grid)
        {
            grid.InitGrid();
            _leftBound = -grid.GridWidth / 2;
            _rightBound = -_leftBound;
            _topBound = grid.GridHeight / 2;
            _botBound = -_topBound;

        }
    }
    private void InitLists()
    {
        //init lists
        _movingColliders = new List<SATCollider>();
        _PhysicsGameObjects = new List<GameObject>();
        _CollidedObjects = new List<GameObject>();
        _physicsComponents = new List<PhysicsComponent>();
        _sphereColliders = new List<SpherePhysicsComponent>();
        _lineColliders = new List<LineCollider>();
        _gameObjectPhysicsComponentMap = new Dictionary<GameObject, PhysicsComponent>();
        _CollidedObjectsMap = new Dictionary<GameObject, CollisionInfo>();
        _SATColliders = new List<SATCollider>();
    }
    private void LoadPhysicsObjects()
    {

        //get all tagged gameObjects
        GameObject[] newObjects = GameObject.FindGameObjectsWithTag("PhysicsObject");
        _PhysicsGameObjects = newObjects.ToList();

        //store components in corresponding lists
        foreach (GameObject otherGameObject in _PhysicsGameObjects)
        {
            if (otherGameObject.GetComponent<PhysicsComponent>())
            {
                PhysicsComponent newComponent = otherGameObject.GetComponent<PhysicsComponent>();
                _physicsComponents.Add(newComponent);
                _gameObjectPhysicsComponentMap.Add(otherGameObject, newComponent);
                newComponent.InitPhysicsComponent();
                switch (newComponent)
                {
                    case SpherePhysicsComponent c:
                        _sphereColliders.Add(c);
                        break;
                    case LineCollider c:
                        _lineColliders.Add(c);
                        break;
                    case SATCollider c:
                        _SATColliders.Add(c);
                        break;
                }
            }
            else
            {
                Debug.LogWarning("GameObject Tagged as physics object is missing physics component");
            }
        }
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
        //  Debug.Log("components :" + _physicsComponents.Count);
        foreach (PhysicsComponent component in _physicsComponents)
        {
            if (!component.IsStatic()) component.UpdateComponent();
            ReflectOnBorders(component);
        }
    }
    //oversimplified reflection for debugging
    private void ReflectOnBorders(PhysicsComponent component)
    {
        float radius = component.Info.radius;
        Vector2 pos = component.Info.NewPosition;

        if (pos.x + radius > _rightBound-1)
        {
            component.Info.Direction = Vector2.Reflect(component.Info.Direction, Vector2.left);
        }

        if (pos.x - radius < _leftBound+1)
        {
            component.Info.Direction = Vector2.Reflect(component.Info.Direction, Vector2.right);
        }

        if (pos.y + radius > _topBound-1)
        {
            component.Info.Direction = Vector2.Reflect(component.Info.Direction, Vector2.down);
        }
        if (pos.y - radius < _botBound+1)
        {
            component.Info.Direction = Vector2.Reflect(component.Info.Direction, Vector2.up);
        }
    }
    private void StartUpdate()
    {

        grid.UpdateColliders(_movingColliders);

        foreach (PhysicsComponent component in _physicsComponents)
        {
            //isStatic always returns false if it is not overwritten
            if (!component.IsStatic()) { component.Step(); }
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
        //SphereSphereCollision();
        //SphereLineCollision();
        // SATCollision();
        GridSATCollision();
        //does only detect collision for corner points of box
        //BoxSphereCollision();
    }

    private void BoxSphereCollision()
    {
        foreach (SATCollider Box in _SATColliders)
        {
            foreach (SpherePhysicsComponent Sphere in _sphereColliders)
            {
                CollisionInfo inf = SpherBoxIntersection(Sphere, Box);
                if (inf.hit)
                {
                    _CollidedObjects.Add(Sphere.gameObject);
                    _CollidedObjects.Add(Box.gameObject);
                }
            }
        }
    }
    private void GridSATCollision()
    {
        foreach (ComponentContainer sortedGrid in grid.GetSortedGrid())
        {
            List<SATCollider> colliders = sortedGrid.list;
            for (int i = 0; i < colliders.Count; i++)
            {
                for (int j = i; j < colliders.Count; j++)
                {
                    CollisionInfo inf = SATIntersection(colliders[i], colliders[j]);
                    if (inf.hit)
                    {
                        _CollidedObjects.Add(colliders[i].gameObject);
                        _CollidedObjects.Add(colliders[j].gameObject);
                    }
                }
            }
        }
    }
    private void SATCollision()
    {
        for (int i = 0; i < _SATColliders.Count; i++)
        {
            for (int j = i; j < _SATColliders.Count; j++)
            {
                CollisionInfo inf = SATIntersection(_SATColliders[i], _SATColliders[j]);
                if (inf.hit)
                {
                    _CollidedObjects.Add(_SATColliders[i].gameObject);
                    _CollidedObjects.Add(_SATColliders[j].gameObject);
                }
                else
                {
                }
            }
        }
        //foreach (SATCollider currentBoxCollider in _SATColliders)
        //{
        //    foreach (SATCollider otherBoxCollider in _SATColliders)
        //    {
        //        if (otherBoxCollider != currentBoxCollider)
        //        {
        //            CollisionInfo inf = SATIntersection(currentBoxCollider, otherBoxCollider);
        //            if (inf.hit)
        //            {
        //                _CollidedObjects.Add(currentBoxCollider.gameObject);
        //                _CollidedObjects.Add(otherBoxCollider.gameObject);
        //            }
        //            else
        //            {
        //                Debug.Log("no collision");
        //            }
        //        }
        //    }
        //}

    }

    private CollisionInfo SpherBoxIntersection(SpherePhysicsComponent sphere, SATCollider box)
    {
        CollisionInfo info = new CollisionInfo();
        Vector2 pointA = box.GetOldPosition() + new Vector2(-box.radiusX, -box.radiusY);
        Vector2 pointB = box.GetOldPosition() + new Vector2(box.radiusX, -box.radiusY);
        Vector2 pointC = box.GetOldPosition() + new Vector2(-box.radiusX, box.radiusY);
        Vector2 pointD = box.GetOldPosition() + new Vector2(box.radiusX, box.radiusY);

        if ((sphere.GetPosition() - pointA).magnitude < sphere.GetRadius())
        {
            info.hit = true;
            return info;
        }
        if ((sphere.GetPosition() - pointB).magnitude < sphere.GetRadius())
        {
            info.hit = true;
            return info;
        }
        if ((sphere.GetPosition() - pointC).magnitude < sphere.GetRadius())
        {
            info.hit = true;
            return info;
        }
        if ((sphere.GetPosition() - pointD).magnitude < sphere.GetRadius())
        {
            info.hit = true;
            return info;
        }

        return info;
    }
    private CollisionInfo SATIntersection(SATCollider a, SATCollider b)
    {
        CollisionInfo info = new CollisionInfo();
        foreach (Axis currentAxis in a.Axises)
        {
            Vector2 axis = currentAxis.AxisNormal;
            //returns the minimum and maximum Dot product between the axis and all the verticies
            Tuple<float, float> minMaxA = ProjectShape.Project(axis, a.Info.verticies);
            Tuple<float, float> minMaxB = ProjectShape.Project(axis, b.Info.verticies);

            if (!ProjectShape.Overlap(minMaxA, minMaxB))
            {
                //stop collision detection if no Overlap was calculated
                info.hit = false;
                return info;
            }
        }
        foreach (Axis currentAxis in b.Axises)
        {

            Vector2 axis = currentAxis.AxisNormal;
            Tuple<float, float> minMaxA = ProjectShape.Project(axis, a.Info.verticies);
            Tuple<float, float> minMaxB = ProjectShape.Project(axis, b.Info.verticies);
            if (!ProjectShape.Overlap(minMaxA, minMaxB))
            {
                info.hit = false;
                return info;
            }
        }


        info.hit = true;
        return info;
    }

    private void DebugCollision()
    {
        foreach (GameObject physicsGameObject in _PhysicsGameObjects)
        {
            if (_CollidedObjects.Contains(physicsGameObject))
            {
                physicsGameObject.GetComponent<SpriteRenderer>().color = Color.red;
                //   _gameObjectPhysicsComponentMap[physicsGameObject].Info.Speed = 0;
                //physicsGameObject.GetComponent<PhysicsInfo>().NewPosition =
                //    _CollidedObjectsMap[physicsGameObject].PointOfImpact;
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

            float oldDistance = SphereLineDistance(sphereInfo.OldPosition, line);
            float newDistance = SphereLineDistance(sphereInfo.NewPosition, line);
            float a = oldDistance - sphere.GetRadius();
            float b = oldDistance + newDistance;

            float t = a / b;

            Vector2 offset = sphereInfo.Direction.normalized * t * sphereInfo.Velocity;

            pointOfImpact = sphereInfo.OldPosition + offset;

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

public static class ProjectShape
{
    public static Tuple<float, float> Project(Vector2 axis, Vector2[] ShapeVerticies)
    {
        //set min and max to the dot value of the first vertex
        float min;
        float max;
        min = Vector2.Dot(axis, ShapeVerticies[0]);
        max = min;
        //For the rest of the verticies calculate the dot product & store it as min / max if its smaller / larger than min / max
        for (int i = 1; i < ShapeVerticies.Length; i++)
        {
            float currentDot = Vector2.Dot(axis, ShapeVerticies[i]);
            if (currentDot < min)
            {
                min = currentDot;
            }
            else if (currentDot > max)
            {
                max = currentDot;
            }
        }

        Tuple<float, float> returnType = new Tuple<float, float>(min, max);

        return returnType;
    }

    public static bool Overlap(Tuple<float, float> a, Tuple<float, float> b)
    {
        //b min is in between a min and a max --> overlap
        if (a.Item1 < b.Item1 && b.Item1 < a.Item2)
        {
            return true;
        }
        //a min is in between b min and b max --> overlap
        if (a.Item1 > b.Item1 && a.Item1 < b.Item2)
        {
            return true;
        }

        return false;
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