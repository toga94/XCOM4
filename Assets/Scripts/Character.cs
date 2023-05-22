using UnityEngine;

internal struct Character
{
    public Character(Transform transform, Collider collider, Unit unit)
    {
        GetTransform = transform;
        GetCollider = collider;
        GetUnit = unit;
    }

    public readonly Collider GetCollider;
    public readonly Transform GetTransform;
    public readonly Unit GetUnit;
}