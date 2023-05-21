using DG.Tweening;
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

    public override void Cast(GameObject target, float additionalDamage)
    {
        PoolingSystem(nameof(ProjectileBallAbility));
        // Cast fireball spell
        if (target == null) return;
        StartCoroutine(FireballCast(target, additionalDamage));
    }

    private void PoolingSystem(string className)
    {
        if (projectilePool == null)
        {
            animator = GetComponent<Animator>();
            GameObject poolObj = GameObject.Find("_Pooling");

            Transform childTransform = poolObj.transform.Find(className);
            GameObject childObject;

            if (childTransform != null)
            {
                childObject = childTransform.gameObject;
                projectilePool = childObject.GetComponent<LeanGameObjectPool>();
            }
            else
            {
                childObject = new GameObject(className);
                childObject.transform.parent = poolObj.transform;
                projectilePool = childObject.AddComponent<LeanGameObjectPool>();
                projectilePool.Prefab = projectilePrefab;
                projectilePool.Capacity = 10;
                projectilePool.Recycle = true;
            }
        }
    }
    private GameObject backupTarget;
    private float backupAddDamage;
    private IEnumerator FireballCast(GameObject target, float additionalDamage)
    {
        if (target == null)
        {
            yield break; // exit the method if target is null
        }
        backupTarget = target;
        backupAddDamage = additionalDamage;
        Vector3 targetPos = target.transform.position;
        animator.Play(base.abilityType.ToString());
        float height = 3;
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion toTargetQuaternion = Quaternion.LookRotation(direction, Vector3.up);
        GameObject projectile = projectilePool.Spawn(transform.position + Vector3.up * height, toTargetQuaternion);
        float speed = 10f;
        float timeStep = Time.deltaTime;
        projectile.transform.DOMove(targetPos, Vector3.Distance(projectile.transform.position, targetPos) / speed).SetEase(Ease.Linear).OnComplete(() =>
        {
            projectilePool.Despawn(projectile);
            backupTarget.GetComponent<IDamageable>().TakeDamage(AbilityPower + backupAddDamage);
        });
    }
}