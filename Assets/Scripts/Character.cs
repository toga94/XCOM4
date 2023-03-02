using UnityEngine;

internal class Character
{
    public Character(Transform transform, Collider collider, Animator animator, Unit unit)
    {
        this.GetTransform = transform;
        this.GetCollider = collider;
        this.GetAnimator = animator;
        this.GetUnit = unit;
    }

    public Animator GetAnimator { get; }
    public Collider GetCollider { get; }
    public Transform GetTransform { get; }
    public Unit GetUnit { get; }
}