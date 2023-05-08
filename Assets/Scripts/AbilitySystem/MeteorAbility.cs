using Lean.Pool;
using System;
using System.Collections;
using UnityEngine;
public class MeteorAbility : Ability
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
        if (target == null) return;
        StartCoroutine(MeteorCast(target));
    }

    private void PoolingSystem()
    {
        if (projectilePool == null)
        {
            animator = GetComponent<Animator>();
            GameObject poolObj = GameObject.Find("_Pooling");

            Transform childTransform = poolObj.transform.Find(nameof(MeteorAbility));
            GameObject childObject;

            if (childTransform != null)
            {
                childObject = childTransform.gameObject;
                projectilePool = childObject.GetComponent<LeanGameObjectPool>();
            }
            else
            {
                childObject = new GameObject(nameof(MeteorAbility));
                childObject.transform.parent = poolObj.transform;
                projectilePool = childObject.AddComponent<LeanGameObjectPool>();
                projectilePool.Prefab = projectilePrefab;
                projectilePool.Capacity = 10;
                projectilePool.Recycle = true;
            }
        }
    }

    private IEnumerator MeteorCast(GameObject target)
    {
        if (target == null)
        {
            yield break; // exit the method if target is null
        }
        Vector3 targetPos = target.transform.position;
        animator.Play(base.abilityType.ToString());
        // Calculate the direction to the target
        float height = 20;
        Vector3 spawnPosition = targetPos + Vector3.up * height;
        Vector3 direction = (targetPos - spawnPosition).normalized;
        // Calculate the rotation to face the target
        Quaternion to_Target_Quaternion = Quaternion.LookRotation(direction, Vector3.up);
        GameObject projectile = projectilePool.Spawn(spawnPosition, to_Target_Quaternion);
        Vector3 projectilePos = projectile.transform.position;
        // Move the projectile towards the target
        float speed = 50f; // Speed of the projectile
        while (Vector3.Distance(projectilePos, targetPos) > 0f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectilePos, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        projectilePool.Despawn(projectile);
    }
}