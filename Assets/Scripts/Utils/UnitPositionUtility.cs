using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class UnitPositionUtility
{
    public static List<TransformData> SaveUnitsPosition()
    {
        GameManager gameManager = GameManager.Instance;
        LevelGrid levelGrid = LevelGrid.Instance;
        List<Unit> unitsOnGrid = gameManager.GetAllUnitsOnGrid;
        List<TransformData> unitTransforms = unitsOnGrid.Select(unit =>
            new TransformData(
                new Vector3(levelGrid.GetWorldPosition(unit.UnitGridPosition).x, levelGrid.GetWorldPosition(unit.UnitGridPosition).y, levelGrid.GetWorldPosition(unit.UnitGridPosition).z),
                unit.transform.rotation)).ToList();

        gameManager.SavedUnitTransforms = unitTransforms;
        return unitTransforms;
    }

    public static void LoadUnitsPosition()
    {
        GameManager gameManager = GameManager.Instance;
        List<Unit> unitsOnGrid = gameManager.GetAllUnitsOnGrid;
        unitsOnGrid.Select((unit, index) => new { unit, index }).ToList().ForEach(obj =>
        {
            obj.unit.transform.SetPositionAndRotation(
                gameManager.SavedUnitTransforms[obj.index].position, gameManager.SavedUnitTransforms[obj.index].rotation);
        });
    }


    public static void RefreshUnitsPosition() {
        SaveUnitsPosition();
        LoadUnitsPosition();
    }
}