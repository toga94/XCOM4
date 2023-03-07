using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName;
    [SerializeField] private int unitLevel;
    public int type; // Type of unit (e.g. archer, knight, mage)
    public int tier; // Tier of unit (e.g. 1-star, 2-star, 3-star)
    public bool OnGrid;

    public string GetUnitName { get => unitName + unitLevel; }

    public void Move(Vector3 targetPosition) => transform.position = targetPosition;

    public bool CanMergeWith(Unit otherUnit)
    {
        // Check if this unit can merge with another unit based on type and tier
        return (this.type == otherUnit.type && this.tier == otherUnit.tier);
    }

    public void MergeWith(List<Unit> otherUnits)
    {
        // Create new merged unit object and replace the original units with the new object in the grid
        Unit mergedUnit = new Unit();
        mergedUnit.type = this.type;
        mergedUnit.tier = this.tier + 1; // Increment the tier for the new merged unit
        // Set other merged unit properties (e.g. health, attack, etc.)
        // Replace the original units in the grid with the new merged unit
        // Update any relevant UI or game state information to reflect the merge
    }
}
