using System.Collections;
using UnityEngine;
using DG.Tweening;
using Lean.Pool;

public class ProjectileBallAbility : Ability
{
    private Animator animator;
    public string AnimationName = "fireball";
    [SerializeField] private GameObject projectilePrefab;
    private LeanGameObjectPool projectilePool;

    private GameObject backupTarget;
    private float backupAddDamage;
    [SerializeField] private bool CustomAnimation;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Cast(GameObject target, float additionalDamage)
    {
        if (target == null) return;

        PoolingSystem(projectilePrefab.name);
        StartCoroutine(FireballCast(target, additionalDamage));
    }

    private void PoolingSystem(string className)
    {
        if (projectilePool == null)
        {
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

    private IEnumerator FireballCast(GameObject target, float additionalDamage)
    {
        backupTarget = target;
        backupAddDamage = additionalDamage;
        Vector3 targetPos = backupTarget.transform.position;
        if (!CustomAnimation) animator.Play(base.abilityType.ToString());
        else {
            animator.Play(base.AbilityName);
        }
        float height = 3;
        Vector3 direction = (targetPos - transform.position).normalized;
        Quaternion toTargetQuaternion = Quaternion.LookRotation(direction, Vector3.up);
        GameObject projectile = projectilePool.Spawn(transform.position + Vector3.up * height, toTargetQuaternion);
        float speed = 10f;
        float distance = Vector3.Distance(projectile.transform.position, targetPos);
        float duration = distance / speed;

        yield return projectile.transform.DOMove(targetPos, duration).SetEase(Ease.Linear).WaitForCompletion();

        projectilePool.Despawn(projectile);

        float damage = AbilityPower + backupAddDamage;
        int totalDamage = Mathf.FloorToInt(UnityEngine.Random.Range(damage, damage * 2f));

        bool isCritical = totalDamage > damage * 1.6f;

        if (backupTarget != null && backupTarget.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(totalDamage, isCritical);
        }



    }
}
