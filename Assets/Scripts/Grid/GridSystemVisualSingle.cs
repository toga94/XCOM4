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
    private GridPosition gridPosition;
    [SerializeField] private List<Unit> unityList;
    private void Start()
    {

        levelGrid = LevelGrid.Instance;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += OnAnyUnitMovedGridPosition;
        gridPosition = levelGrid.GetGridPosition(transform.position);
    }

    private void OnApplicationQuit()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= OnAnyUnitMovedGridPosition;
    }
    private void OnDestroy()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= OnAnyUnitMovedGridPosition;
    }
    private void OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        string debugText;
        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
        {
            debugText = $"{gridPosition.ToString()} \n {LevelGrid.Instance.GetUnitAtGridPosition(gridPosition).GetUnitName}";
        }
        else {
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