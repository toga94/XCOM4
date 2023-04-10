using System.Collections.Generic;
using UnityEngine;

public class CombatPhaseState : GameState
{
    private List<TransformData> unitTransforms = new List<TransformData>();

    // Logic for entering Combat Phase state
    public override void OnEnterState()
    {
        GameManager gm = GameManager.Instance;
        List<Unit> units = GameManager.Instance.GetAllUnitsOnGrid;
        if (gm == null)
        {
            Debug.LogError("GameManager instance is null!");
            return;
        }
        // Save all unit transforms
        unitTransforms.Clear();
        foreach (Unit unit in units)
        {
            unitTransforms.Add(new TransformData(unit.transform.position, unit.transform.rotation));
        }
        GameManager.Instance.SavedUnitTransforms = unitTransforms;
    }

    // Logic for updating Combat Phase state
    public override void OnUpdate()
    {
        // TODO: Add combat logic
    }

    // Logic for exiting Combat Phase state
    public override void OnExitState()
    {
        GameManager gm = GameManager.Instance;
        List<Unit> units = GameManager.Instance.GetAllUnitsOnGrid;
        List<TransformData> savedTransforms = GameManager.Instance.SavedUnitTransforms;

        // Move units back to saved positions and rotations
        for (int i = 0; i < units.Count; i++)
        {
            units[i].transform.position = savedTransforms[i].position;
            units[i].transform.rotation = savedTransforms[i].rotation;
        }
    }
}