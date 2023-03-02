using System;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    private TGridObject[,] gridObjectArray;
    private Vector3 GlobalOffset = new Vector3(-4, 0, -6);
  
    public GridSystem(int width, int height, float cellSize, Func<GridSystem<TGridObject>, GridPosition, TGridObject> createGridObject, Vector3Int offset)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                gridObjectArray[x, z] = createGridObject(this, gridPosition);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * cellSize + GlobalOffset;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
       Vector3 curWorldPosition  = worldPosition - GlobalOffset;
        return new GridPosition(
            Mathf.RoundToInt(curWorldPosition.x / cellSize) ,
            Mathf.RoundToInt(curWorldPosition.z / cellSize)
        );
    }

    public void CreateDebugObjects(Transform debugPrefab)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);

                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return gridObjectArray[gridPosition.x, gridPosition.z];
    }

    public bool IsValidGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 &&
                gridPosition.z >= 0 &&
                gridPosition.x < width &&
                gridPosition.z < height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetHeight()
    {
        return height;
    }



}
