using System.Collections;
using UnityEngine;
using DG.Tweening;
using Lean.Pool;

public class RifleRapidFireAbility : Ability
{
    private Animator animator;
    public string AnimationName = "rifleShoot";
    [SerializeField] private GameObject projectilePrefab;
    private LeanGameObjectPool projectilePool;

    private GameObject backupTarget;
    private float backupAddDamage;
    [SerializeField] private bool CustomAnimation;
    private int numberOfProjectiles = 20; // Number of projectiles per shot
    private float fireRate = 1f / 20f; // Fire rate (20 projectiles per second)

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Cast(GameObject target, float additionalDamage)
    {
        if (target == null) return;

        PoolingSystem(projectilePrefab.name);
        StartCoroutine(RifleRapidFire(target, additionalDamage));
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
                projectilePool.Capacity = 20;
                projectilePool.Recycle = true;
            }
        }
    }

    private IEnumerator RifleRapidFire(GameObject target, float additionalDamage)
    {
        backupTarget = target;
        backupAddDamage = additionalDamage;
        Vector3 targetPos = backupTarget.transform.position;

        if (!CustomAnimation) animator.Play(base.abilityType.ToString());
        else animator.Play(base.AbilityName);

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            // Spawn each projectile
            FireProjectile(targetPos);

            // Wait for the next shot
            yield return new WaitForSeconds(fireRate);
        }
    }
    private void FireProjectile(Vector3 targetPos)
    {
        float height = 3;
        Vector3 direction = (targetPos - transform.position).normalized;

        // Add spread by applying a small random angle to the direction
        float spreadAngle = 10f; // Adjust this to control the spread (in degrees)
        direction = Quaternion.Euler(
            Random.Range(-spreadAngle, spreadAngle),
            Random.Range(-spreadAngle, spreadAngle),
            0
        ) * direction;

        Quaternion toTargetQuaternion = Quaternion.LookRotation(direction, Vector3.up);

        GameObject projectile = projectilePool.Spawn(transform.position + Vector3.up * height, toTargetQuaternion);
        float speed = 10f;
        float distance = Vector3.Distance(projectile.transform.position, targetPos);
        float duration = distance / speed;

        projectile.transform.DOMove(targetPos, duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            projectilePool.Despawn(projectile);

            float damage = AbilityPower + backupAddDamage;
            float totalDamage = Random.Range(damage, damage * 2f) / 10;

            bool isCritical = totalDamage > damage * 1.6f;

            if (backupTarget != null && backupTarget.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(totalDamage, isCritical);
            }
        });
    }
}