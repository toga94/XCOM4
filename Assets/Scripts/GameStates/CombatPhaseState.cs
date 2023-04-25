using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatPhaseState : GameState
{
    private List<TransformData> unitTransforms = new List<TransformData>();

    // Logic for entering Combat Phase state
    public override void OnEnterState()
    {
        GameManager gm = GameManager.Instance;
        if (gm == null)
        {
            Debug.LogError("GameManager instance is null!");
            return;
        }

        // Save all unit transforms
        unitTransforms = gm.GetAllUnitsOnGrid.Select(unit => new TransformData(unit.transform.position, unit.transform.rotation)).ToList();
        gm.SavedUnitTransforms = unitTransforms;

        // Disable all box colliders with tag "floor"
        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        floors.Select(floor => floor.GetComponent<BoxCollider>()).ToList().ForEach(collider => collider.enabled = false);
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
        List<Unit> units = gm.GetAllUnitsOnGrid;
        List<TransformData> savedTransforms = gm.SavedUnitTransforms;
        // Move units back to saved positions and rotations
        units.Select((unit, index) => new { unit, index }).ToList().ForEach(obj => {
            obj.unit.transform.position = savedTransforms[obj.index].position;
            obj.unit.transform.rotation = savedTransforms[obj.index].rotation;
        });

        GameObject[] floors = GameObject.FindGameObjectsWithTag("floor");
        floors.Select(floor => floor.GetComponent<BoxCollider>()).ToList().ForEach(bc => bc.enabled = true);
    }
}