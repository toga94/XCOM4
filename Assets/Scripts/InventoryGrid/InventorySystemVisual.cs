using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystemVisual : MonoBehaviour
{

    public static InventorySystemVisual Instance { get; private set; }
    private InventoryGrid inventoryGrid;

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

    [SerializeField] private Transform inventorySystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;


    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one InventorySystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        inventoryGrid = InventoryGrid.Instance;
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            inventoryGrid.GetWidth(),
            1
        ];

        for (int x = 0; x < inventoryGrid.GetWidth(); x++)
        {
            GridPosition gridPosition = new GridPosition(x, 0);
            Vector3 offset = new Vector3(-2.5f, 0f, 2.5f);
            Transform gridSystemVisualSingleTransform =
                Instantiate(inventorySystemVisualSinglePrefab, InventoryGrid.Instance.GetInventoryWorldPosition(gridPosition), Quaternion.identity);

            gridSystemVisualSingleArray[x, 0] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
        }

        inventoryGrid.OnAnyUnitMovedInventoryPosition += InventoryGrid_OnAnyUnitMovedinventoryPosition;

        UpdateInventoryGridVisual();


    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < inventoryGrid.GetWidth(); x++)
        {
            for (int z = 0; z < inventoryGrid.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
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

        //        if (!inventoryGrid.IsValidInventoryPosition(testGridPosition))
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
    public void HideGridPositionList(List<GridPosition> gridPositionList)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].
               Hide();
        }
    }

    public void UpdateInventoryGridVisual()
    {
        //ShowGridPositionRange(new GridPosition(0, 0), 1000, GridVisualType.RedSoft);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateInventoryGridVisual();
    }

    private void InventoryGrid_OnAnyUnitMovedinventoryPosition(object sender, EventArgs e)
    {
        UpdateInventoryGridVisual();
    }

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