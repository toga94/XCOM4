using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{

    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private TextMeshPro textmeshPro;
    private GridObject gridObject;
    private LevelGrid levelGrid;
    private InventoryGrid inventoryGrid;
    private GridPosition gridPosition;
    [SerializeField] private List<Unit> unityList;

    public bool isInventory;
    private void Start()
    {
        
        isInventory = this.name.Contains("Inventory");
        if (!isInventory)
        {
            levelGrid = LevelGrid.Instance;
            LevelGrid.Instance.OnAnyUnitMovedGridPosition += OnAnyUnitMovedGridPosition;
            gridPosition = levelGrid.GetGridPosition(transform.position);
        }
        else
        {
            inventoryGrid = InventoryGrid.Instance;
            InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += OnAnyUnitMovedGridPosition;
            gridPosition = inventoryGrid.GetInventoryPosition(transform.position);
        }
        GameManager.Instance.OnUpdateText += UpdateText;
    }

    private void OnApplicationQuit()
    {
        if (!isInventory)
        {
            LevelGrid.Instance.OnAnyUnitMovedGridPosition -= OnAnyUnitMovedGridPosition;
        }
        else
        {
            InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition -= OnAnyUnitMovedGridPosition;
        }
    }
    private void OnDestroy()
    {
        if (!isInventory)
        {
            LevelGrid.Instance.OnAnyUnitMovedGridPosition -= OnAnyUnitMovedGridPosition;
        }
        else
        {
            InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition -= OnAnyUnitMovedGridPosition;
        }
    }

    private void UpdateText(object sender, GameManager.UpdateTextArg e)
    {
        string debugText;
        if (!isInventory)
            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
            {
                debugText = $"{gridPosition.ToString()} \n {LevelGrid.Instance.GetUnitAtGridPosition(gridPosition).GetUnitName}";
            }
            else
            {
                debugText = gridPosition.ToString();
            }
        else
        {
            if (InventoryGrid.Instance.HasAnyUnitOnInventoryPosition(gridPosition))
            {
                debugText = $"{gridPosition.ToString()} \n {InventoryGrid.Instance.GetUnitAtInventoryPosition(gridPosition).GetUnitName}";
            }
            else
            {
                debugText = gridPosition.ToString();
            }
        }
        textmeshPro.text = debugText;
    }

    private void OnAnyUnitMovedGridPosition(object sender, InventoryGrid.OnAnyUnitMovedInventoryPositionEventArgs e)
    {
        if (!isInventory) return;
        string debugText;
        if (InventoryGrid.Instance.HasAnyUnitOnInventoryPosition(gridPosition))
        {
            debugText = $"{gridPosition.ToString()} \n {InventoryGrid.Instance.GetUnitAtInventoryPosition(gridPosition).GetUnitName}";
        }
        else
        {
            debugText = gridPosition.ToString();
        }
        textmeshPro.text = debugText;
    }
    private void OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        if (isInventory) return;
        string debugText;
        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
        {
            debugText = $"{gridPosition.ToString()} \n {LevelGrid.Instance.GetUnitAtGridPosition(gridPosition).GetUnitName}";
        }
        else
        {
            debugText = gridPosition.ToString();
        }
        textmeshPro.text = debugText;
    }

    public void SetDebugObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
    }

    public void Show(Material material)
    {
        meshRenderer.enabled = true;
        meshRenderer.material = material;
    }

    public void Hide()
    {
        meshRenderer.enabled = false;
    }

}