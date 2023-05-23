using UnityEngine;

public class RoundBoughtBuilder
{
    private RoundBought roundBought;

    public RoundBoughtBuilder()
    {
        roundBought = new RoundBought();
    }

    public RoundBoughtBuilder AddGridUnit(Unit unit)
    {
        roundBought.gridUnits.Add(unit);
        return this;
    }

    public RoundBoughtBuilder AddGridUnitPosition(Vector3 position)
    {
        roundBought.gridUnitsPositions.Add(position);
        return this;
    }

    public RoundBoughtBuilder AddInventoryUnit(Unit unit)
    {
        roundBought.inventoryUnits.Add(unit);
        return this;
    }

    public RoundBoughtBuilder AddInventoryUnitPosition(Vector3 position)
    {
        roundBought.inventoryUnitsPosition.Add(position);
        return this;
    }

    public RoundBought Build()
    {
        return roundBought;
    }
}
