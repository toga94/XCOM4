using Lean.Pool;
using System;
using System.Collections;
using UnityEngine;
public class ProjectileBallAbility : Ability
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
        StartCoroutine(FireballCast(target));
    }

    private void PoolingSystem()
    {
        if (projectilePool == null)
        {
            animator = GetComponent<Animator>();
            GameObject poolObj = GameObject.Find("_Pooling");

            var childTransform = poolObj.transform.Find(nameof(ProjectileBallAbility));
            GameObject childObject;

            if (childTransform != null)
            {
                childObject = childTransform.gameObject;
                projectilePool = childObject.GetComponent<LeanGameObjectPool>();
            }
            else
            {
                childObject = new GameObject(nameof(ProjectileBallAbility));
                childObject.transform.parent = poolObj.transform;
                projectilePool = childObject.AddComponent<LeanGameObjectPool>();
                projectilePool.Prefab = projectilePrefab;
                projectilePool.Capacity = 10;
                projectilePool.Recycle = true;
            }
        }
    }

    private IEnumerator FireballCast(GameObject target)
    {
        animator.Play(base.abilityType.ToString());
        float height = 3;
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion toTargetQuaternion = Quaternion.LookRotation(direction, Vector3.up);
        GameObject projectile = projectilePool.Spawn(transform.position + Vector3.up * height, toTargetQuaternion);
        float speed = 10f;
        float timeStep = Time.deltaTime;

        while (Vector3.Distance(projectile.transform.position, target.transform.position) > 0f)
        {
            projectile.transform.position = Vector3.MoveTowards(projectile.transform.position, target.transform.position, speed * timeStep);
            yield return null;
        }

        projectilePool.Despawn(projectile);
    }
}