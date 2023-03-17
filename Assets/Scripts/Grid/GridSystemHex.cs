using System;
using UnityEngine;

public class GridSystemHex<TGridObjectHex>
{

    private  float HEX_VERTICAL_OFFSET_MULTIPLIER = 0.75f;
    private  float HEX_HORIZONTAL_OFFSET_MULTIPLIER = 0.75f;


    private int width;
    private int height;
    private float cellSize;
    private TGridObjectHex[,] gridObjectArray;

    public GridSystemHex(int width, int height, float cellSize, Func<GridSystemHex<TGridObjectHex>, GridPosition, TGridObjectHex> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        gridObjectArray = new TGridObjectHex[width, height];

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
        return
            new Vector3(gridPosition.x, 0, 0) * cellSize  +
            new Vector3(0, 0, gridPosition.z) * cellSize * HEX_VERTICAL_OFFSET_MULTIPLIER +
            (((gridPosition.z % 2) == 1) ? new Vector3(1, 0, 0) * cellSize * -0.5001f : Vector3.zero);
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x / cellSize);
        int z = Mathf.RoundToInt(worldPosition.z / (cellSize * HEX_VERTICAL_OFFSET_MULTIPLIER));

        if (z % 2 == 1)
        {
            int closestX = x;
            float closestDistance = float.MaxValue;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    int nx = x + dx;
                    int nz = z + dz;
                    if (Mathf.Abs(dx) == 1 && dz == 0 || dx == 0 && Mathf.Abs(dz) == 1 || dx == -1 && dz == -1)
                    {
                        float distance = Vector3.Distance(worldPosition, GetWorldPosition(new GridPosition(nx, nz)));
                        if (distance < closestDistance)
                        {
                            closestDistance = distance;
                            closestX = nx;
                        }
                    }
                }
            }

            return new GridPosition(closestX, z);
        }
        else
        {
            return new GridPosition(x, z);
        }
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
                gridDebugObject.SetGridObject(GetGridObjectHex(gridPosition));
            }
        }
    }

    public TGridObjectHex GetGridObjectHex(GridPosition gridPosition)
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
