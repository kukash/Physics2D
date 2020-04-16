using System;
using UnityEngine;
using System.Collections.Generic;
using UnityScript.Steps;

public class PartitioningGrid : MonoBehaviour
{

    public int CellSize = 5;
    public int GridWidth = 40;
    public int GridHeight = 25;
    public Color LineColor = Color.red;
    private int xCount = 0;
    private int yCount = 0;

    private int collums = 0;
    private int rows = 0;
    // private const float visualOffsetX = 0.254f;
    private const float visualOffsetX = -0.5f;

    // private const float visualOffsetY = -0.31f;
    private const float visualOffsetY = 0.5f;

    //this stores Components
    private ComponentContainer[,] ComponentGrid;
    private List<GridCell> _cells;


    public void InitGrid()
    {

        Vector3 startPos = new Vector3(-GridWidth / 2 + visualOffsetX, GridHeight / 2 + visualOffsetY, 0);

        Vector3 right = new Vector3(1, 0, 0);
        Vector3 Down = new Vector3(0, -1, 0);



        xCount = GridWidth / CellSize;
        yCount = GridHeight / CellSize;
        ComponentGrid = new ComponentContainer[xCount, yCount];
        for (int i = 0; i < xCount; i++)
        {
            for (int j = 0; j < yCount; j++)
            {
                ComponentGrid[i, j] = new ComponentContainer();
            }
        }

        //Visualization
        //draw lines from top to bottom of grid
        for (int i = 0; i <= xCount; i++)
        {
            Vector3 currentPos = startPos + right * i * CellSize;
            Debug.DrawLine(currentPos, currentPos + Down * GridHeight, LineColor, 100000);

        }

        //Draw lines from left to right of grid
        for (int i = 0; i <= yCount; i++)
        {
            Vector3 currentPos = startPos + Down * i * CellSize;
            Debug.DrawLine(currentPos, currentPos + right * GridWidth, LineColor, 100000);
        }


        //create actual grid
        _cells = new List<GridCell>();
        Vector2 pos = new Vector2(0, 0);
        Vector2 offset = new Vector2(CellSize, -CellSize);

        for (int x = 0; x <= xCount; x++)
        {
            for (int y = 0; y <= yCount; y++)
            {
                pos = new Vector2(x * CellSize - GridWidth / 2, y * -CellSize + GridHeight / 2);
                GridCell newCell = new GridCell(pos, pos + offset);
                _cells.Add(newCell);
            }
        }

    }

    public ComponentContainer[,] GetSortedGrid()
    {
        return ComponentGrid;
    }

    public void InitialPhysicsComponentSort(List<SATCollider> components)
    {




        collums = GridWidth / CellSize;
        rows = GridHeight / CellSize;
        foreach (PhysicsComponent currentComponent in components)
        {
            if (currentComponent is SATCollider)
            {


                SATCollider collider = currentComponent as SATCollider;
                collider.Info.gridPos = new List<Vector2Int>();


                //Foreach cell check if any object points are within grid
                foreach (GridCell cell in _cells)
                {
                    foreach (Vector2 pos in collider.Info.verticies)
                    {
                        int collum = Mathf.FloorToInt((pos.x - GridWidth * 0.5f) / CellSize);
                        collum += collums;
                        int row = Mathf.FloorToInt((pos.y - GridHeight * 0.5f) / CellSize);
                        row += rows;

                        if (!ComponentGrid[collum, row].list.Contains(collider))
                        {
                            ComponentGrid[collum, row].list.Add(collider);
                            collider.Info.gridPos.Add(new Vector2Int(collum, row));
                        }

                    }
                }
            }
        }

        int indexer = 0;
        foreach (var VARIABLE in ComponentGrid)
        {
            //Debug.Log("box" + indexer);
            //Debug.Log("contains :" + VARIABLE.list.Count + "objects");
            ++indexer;
        }

    }

    public void UpdateColliders(List<SATCollider> movedObjects)
    {
      //  Debug.Log("count" + movedObjects.Count);
        foreach (SATCollider movedObject in movedObjects)
        {
            //remove from all cells
            foreach (Vector2Int pos in movedObject.Info.gridPos)
            {
                ComponentGrid[pos.x, pos.y].list.Remove(movedObject);
            }
            movedObject.Info.gridPos.Clear();
            List<Vector2Int> removePos = new List<Vector2Int>();

            //add again
            foreach (Vector2 pos in movedObject.Info.verticies)
            {
                int collum = Mathf.FloorToInt((pos.x - GridWidth * 0.5f) / CellSize);
                collum += collums;
                int row = Mathf.FloorToInt((pos.y - GridHeight * 0.5f) / CellSize);
                row += rows;
                if (!ComponentGrid[collum, row].list.Contains(movedObject))
                {
                    ComponentGrid[collum, row].list.Add(movedObject);
                    movedObject.Info.gridPos.Add(new Vector2Int(collum, row));
                    // Debug.Log("Grid pos" + new Vector2Int(collum, row));
                }
            }
        }

        //int index = 0;
        //foreach (var componentContainer in ComponentGrid)
        //{
        //    index += componentContainer.list.Count;
        //}
        //Debug.Log("count" + index);
    }
}

public class ComponentContainer
{
    public List<SATCollider> list = new List<SATCollider>();
}
public class GridCell
{
    public float radius;
    public Vector2[] CornerPoints = new Vector2[4];
    public List<SATCollider> Colliders = new List<SATCollider>();
    private float _minX = 0;
    private float _maxX = 0;
    private float _minY = 0;
    private float _maxY = 0;
    private Vector2 topLeft;
    private Vector2 botRight;
    public GridCell(Vector2 TopLeft, Vector2 BotRight)
    {
        topLeft = TopLeft;
        botRight = BotRight;
        _minX = topLeft.x;
        _maxX = botRight.x;
        _minY = botRight.y;
        _maxY = topLeft.y;
    }
    public GridCell(float newMinx, float newMaxX, float newMinY, float newMaxY)
    {
        _minX = newMinx;
        _maxX = newMaxX;
        _minY = newMinY;
        _maxY = newMaxY;
    }

    public GridCell(GridCell copyCell)
    {
        _minX = copyCell._minX;
        _maxX = copyCell._maxX;
        _minY = copyCell._minY;
        _maxY = copyCell._maxY;
    }

    public bool PosInGridCheck(Vector2 position)
    {


        if (position.x > _minX && position.x < _maxX && position.y > _minY && position.y < _maxY)
        {
            return true;
        }
        return false;
    }

    public override string ToString()
    {
        string returnString;
        returnString = "TopLeftPos : " + (topLeft + new Vector2(+12, +13)) + "; BotRightPos : " + (botRight + new Vector2(+12, +13));


        return returnString;
    }
}


