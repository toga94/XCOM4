using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GridObjectHex : MonoBehaviour
{

    private GridSystemHex<GridObjectHex> gridSystem;
    private GridPosition gridPosition;
    private List<Unit> unitList;

    public GridObjectHex(GridSystemHex<GridObjectHex> gridSystem, GridPosition gridPosition)
    {
        this.gridSystem = gridSystem;
        this.gridPosition = gridPosition;
        unitList = new List<Unit>();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(gridPosition.ToString()).Append("\n");

        foreach (Unit unit in unitList)
        {
            sb.Append(unit.ToString()).Append("\n");
        }

        return sb.ToString();
    }
    public void RemoveAnyUnit()
    {
        unitList.Clear();
    }
    public void AddUnit(Unit unit)
    {
        unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        unitList.Remove(unit);
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }

    public bool HasAnyUnit()
    {
        return unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit())
        {
            return unitList[0];
        }
        else
        {
            return null;
        }
    }


}
