using UnityEngine;

internal class Character
{
    public Character(Transform transform, Collider collider, Unit unit)
    {
        this.GetTransform = transform;
        this.GetCollider = collider;
        this.GetUnit = unit;
    }
    public Collider GetCollider { get; }
    public Transform GetTransform { get; }
    public Unit GetUnit { get; }
}