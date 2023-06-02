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


    public static List<Vector3> EnemyGridPositions() {
        List<Vector3> enemyGridPositions = new List<Vector3>(28);

        enemyGridPositions.AddRange(new[]
        {
            //1st row
            new Vector3(-0.53f, 0, 15.74f), //0
            new Vector3(4.07f, 0, 15.74f),//1
            new Vector3(8.67f, 0, 15.74f),//2
            new Vector3(13.27f, 0, 15.74f),//3
            new Vector3(17.86f, 0, 15.74f),//4
            new Vector3(22.47f, 0, 15.74f),//5
            new Vector3(27.07f, 0, 15.74f),//6
            //2rd row
            new Vector3(-2.83f, 0, 19.65f),//7
            new Vector3(1.77f, 0, 19.65f),//8
            new Vector3(6.37f, 0, 19.65f),//9
            new Vector3(10.97f, 0, 19.65f),//10
            new Vector3(15.67f, 0, 19.65f),//11
            new Vector3(20.17f, 0, 19.65f),//12
            new Vector3(24.77f, 0, 19.65f),//13
            //3th row
            new Vector3(-0.53f, 0, 23.55f),//14
            new Vector3(4.07f, 0, 23.55f),//15
            new Vector3(8.67f, 0, 23.55f),//16
            new Vector3(13.27f, 0, 23.55f),//17
            new Vector3(17.86f, 0, 23.55f),//18
            new Vector3(22.47f, 0, 23.55f),//19
            new Vector3(27.07f, 0, 23.55f),//20
            //4nd row
            new Vector3(-2.83f, 0, 27.47f),//21
            new Vector3(1.77f, 0, 27.47f),//22
            new Vector3(6.37f, 0, 27.47f),//23
            new Vector3(10.97f, 0, 27.47f),//24
            new Vector3(15.67f, 0, 27.47f),//25
            new Vector3(20.17f, 0, 27.47f),//26
            new Vector3(24.77f, 0, 27.47f)//27
        });
        return enemyGridPositions;
    }



}