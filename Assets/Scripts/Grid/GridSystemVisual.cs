using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{

    public static GridSystemVisual Instance { get; private set; }
    private LevelGrid levelGrid;

    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow,
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;


    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        levelGrid = LevelGrid.Instance;
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            levelGrid.GetWidth(),
            levelGrid.GetHeight()
        ];

        for (int x = 0; x < levelGrid.GetWidth(); x++)
        {
            for (int z = 0; z < levelGrid.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform =
                    Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += LevelGrid_OnAnyUnitMovedGridPosition;


        UpdateGridVisual();
    }
    private void OnDisable()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
        LevelGrid.Instance.OnAnyUnitSwappedGridPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
        InventoryGrid.Instance.OnAnyUnitSwappedInventoryPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition -= LevelGrid_OnAnyUnitMovedGridPosition;
    }
    public void HideAllGridPosition()
    {
        Debug.Log("Hide All grid");
        for (int x = 0; x < levelGrid.GetWidth(); x++)
        {
            for (int z = 0; z < levelGrid.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }
    public void ShowAllGridPosition()
    {
        Debug.Log("Show All grid");
        for (int x = 0; x < levelGrid.GetWidth(); x++)
        {
            for (int z = 0; z < levelGrid.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Show();
            }
        }
    }
    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        //List<GridPosition> gridPositionList = new List<GridPosition>();

        //for (int x = -range; x <= range; x++)
        //{
        //    for (int z = -range; z <= range; z++)
        //    {
        //        GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

        //        if (!levelGrid.IsValidGridPosition(testGridPosition))
        //        {
        //            continue;
        //        }

        //        int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
        //        if (testDistance > range)
        //        {
        //            continue;
        //        }

        //        gridPositionList.Add(testGridPosition);
        //    }
        //}

        //ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].
                Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    public void UpdateGridVisual() => ShowGridPositionRange(new GridPosition(0, 0), 1000, GridVisualType.RedSoft);
    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e) => UpdateGridVisual();
    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e) => UpdateGridVisual();

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }

}