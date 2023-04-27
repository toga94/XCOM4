using Lean.Pool;
using System;
using UnityEngine;
public class FireballAbility : Ability
{
    private Animator animator;
    public string AnimationName = "fireball";
    [SerializeField]
    private GameObject projectilePrefab;
    private LeanGameObjectPool projectilePool;




    public override void Cast(GameObject target)
    {
        PoolingSystem();
        // Cast fireball spell
        FireballCast(target);
    }

    private void PoolingSystem()
    {
        if (projectilePool == null)
        {
            animator = GetComponent<Animator>();
            GameObject poolObj = GameObject.Find("_Pooling");

            var childTransform = poolObj.transform.Find(nameof(FireballAbility));
            GameObject childObject;

            if (childTransform != null)
            {
                childObject = childTransform.gameObject;
                projectilePool = childObject.GetComponent<LeanGameObjectPool>();
            }
            else
            {
                childObject = new GameObject(nameof(FireballAbility));
                childObject.transform.parent = poolObj.transform;
                projectilePool = childObject.AddComponent<LeanGameObjectPool>();
                projectilePool.Prefab = projectilePrefab;
                projectilePool.Capacity = 10;
                projectilePool.Recycle = true;
            }
        }
    }

    private void FireballCast(GameObject target) {
        animator.Play(base.abilityType.ToString());

        // Calculate the direction to the target
        Vector3 direction = (target.transform.position - transform.position).normalized;
        // Calculate the rotation to face the target
        Quaternion to_Target_Quaternion = Quaternion.LookRotation(direction, Vector3.up);
        projectilePool.Spawn(transform.position + Vector3.up * 3, to_Target_Quaternion);
        IDamageable damagableTarget = target.GetComponent<IDamageable>();
        if (damagableTarget == null) return;
        damagableTarget.TakeDamage(base.AbilityPower);

    }
}