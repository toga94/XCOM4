using DG.Tweening;
using Lean.Pool;
using System.Collections;
using UnityEngine;

public class FromGroundAbility : Ability
{
    private Animator animator;
    public string AnimationName = "fireball";
    [SerializeField]
    private GameObject projectilePrefab;
    private LeanGameObjectPool projectilePool;

    public override void Cast(GameObject target, float additionalDamage)
    {
        PoolingSystem(projectilePrefab.name);
        // Cast fireball spell
        if (target == null) return;
        StartCoroutine(SkillCast(target, additionalDamage));
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
    private IEnumerator SkillCast(GameObject target, float additionalDamage)
    {
        backupTarget = target;
        backupAddDamage = additionalDamage;
        Vector3 targetPos = target.transform.position;
        animator.Play(base.abilityType.ToString());
        // Calculate the direction to the target
        float height = 1;
        Vector3 spawnPosition = targetPos - Vector3.up * height;
        Vector3 direction = (targetPos - spawnPosition).normalized;
        // Calculate the rotation to face the target
        Quaternion to_Target_Quaternion = Quaternion.LookRotation(direction, Vector3.up);
        GameObject projectile = projectilePool.Spawn(spawnPosition, to_Target_Quaternion);
        Vector3 projectilePos = target.transform.position - Vector3.up;
        // Move the projectile towards the target
        float speed = 45f; // Speed of the projectile
        float duration = Vector3.Distance(projectilePos, targetPos) / speed;

        float damage = AbilityPower + backupAddDamage;
        int totalDamage = Mathf.FloorToInt(UnityEngine.Random.Range(damage, damage * 2f));

        bool isCritical = totalDamage > damage * 1.6f;

        projectile.transform.DOMove(targetPos, duration).SetEase(Ease.InFlash)
            .OnComplete(() =>
            {
                backupTarget.GetComponent<IDamageable>().TakeDamage(totalDamage, isCritical);
            } );
        yield return new WaitForSeconds(2);
        projectilePool.Despawn(projectile);

        yield return null;
    }
}