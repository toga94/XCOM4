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
    private Material material;

    public bool isInventory;
    private void Start()
    {

        isInventory = this.name.Contains("Inventory");

        levelGrid = LevelGrid.Instance;
        levelGrid.OnAnyUnitMovedGridPosition += OnAnyUnitMovedGridPosition;
        inventoryGrid = InventoryGrid.Instance;
        inventoryGrid.OnAnyUnitMovedInventoryPosition += OnAnyUnitMovedGridPosition;

        if (!isInventory)
        {

            gridPosition = levelGrid.GetGridPosition(transform.position);
        }
        else
        {

            gridPosition = inventoryGrid.GetInventoryPosition(transform.position);
        }
        GameManager.Instance.OnUpdateText += UpdateText;

        material = meshRenderer.material;
    }


    private void UpdateText(object sender, GameManager.UpdateTextArg e)
    {
        textmeshPro.text = isInventory ? InventoryGridUpdate() : LevelGridUpdate();
    }

    private void OnAnyUnitMovedGridPosition(object sender, InventoryGrid.OnAnyUnitMovedInventoryPositionEventArgs e)
    {
        Invoke("UpdateInventoryPosition", 0.1f);
        Invoke("UpdateGridPosition", 0.1f);
    }
    private void OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        Invoke("UpdateGridPosition", 0.1f);
        Invoke("UpdateInventoryPosition", 0.1f);
    }
    private void UpdateInventoryPosition()
    {
        if (!isInventory) return;
        string debugText;
        debugText = InventoryGridUpdate();
        textmeshPro.text = debugText;
    }

    private string InventoryGridUpdate()
    {
        string debugText;
        if (InventoryGrid.Instance.HasAnyUnitOnInventoryPosition(gridPosition))
        {
            debugText = $"{gridPosition.ToString()} \n {InventoryGrid.Instance.GetUnitAtInventoryPosition(gridPosition).GetUnitNameWithLevel}";
            //material.SetFloat("_NegativeAmount", 1f);
        }
        else
        {
            debugText = gridPosition.ToString();
            // material.SetFloat("_NegativeAmount", 0f);
        }

        return debugText;
    }

    private void UpdateGridPosition()
    {
        if (isInventory) return;
        string debugText;
        debugText = LevelGridUpdate();
        textmeshPro.text = debugText;
    }

    private string LevelGridUpdate()
    {
        string debugText;
        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
        {
            debugText = $"{gridPosition.ToString()} \n {LevelGrid.Instance.GetUnitAtGridPosition(gridPosition).GetUnitNameWithLevel}";
            material.SetFloat("_NegativeAmount", 1f);
        }
        else
        {
            debugText = gridPosition.ToString();
            material.SetFloat("_NegativeAmount", 0f);
        }

        return debugText;
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
    public void Show()
    {
        meshRenderer.enabled = true;
    }
    public void Hide()
    {
        meshRenderer.enabled = false;
    }

}