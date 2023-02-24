using UnityEngine;

internal class Character
{
    private Transform transform;
    private Collider collider;
    private Animator animator;
    private Unit unit;

    public Character(Transform transform, Collider collider, Animator animator, Unit unit)
    {
        this.transform = transform;
        this.collider = collider;
        this.animator = animator;
        this.unit = unit;
    }

    public Animator GetAnimator { get => animator; }
    public Collider GetCollider { get => collider; }
    public Transform GetTransform { get => transform; }
    public Unit GetUnit { get => unit; }
}