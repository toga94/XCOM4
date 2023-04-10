using System.Collections.Generic;
using UnityEngine;

public class CombatPhaseState : GameState
{

    // Logic for entering Combat Phase state
    public override void OnEnterState()
    {
        GridSystemVisual.Instance.HideAllGridPosition();
        List<Unit> units = GameManager.Instance.GetAllUnitsOnGrid();

        // Save all unit positions
        unitPositions.Clear();
        foreach (Unit unit in units)
        {
            unitPositions.Add(unit.transform.position);
        }

    }
    // Logic for updating Combat Phase state
    public override void OnUpdate()
    {

    }
    // Logic for exiting Combat Phase state
    public override void OnExitState()
    {

    }
}