using DG.Tweening;
using Lean.Pool;
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
        PoolingSystem(projectilePrefab.name);
        // Cast fireball spell
        if (target == null) return;
        try
        {
            StartCoroutine(FireballCast(target, additionalDamage));
        }
        catch (System.Exception)
        {

            throw;
        } 
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
        float distance;

        distance = Vector3.Distance(projectile != null ? projectile.transform.position : Vector3.down * 55,
            targetPos != null ? targetPos  :Vector3.down * 55);
    float duration = distance / speed;

    projectile.transform.DOMove(targetPos, duration).SetEase(Ease.Linear).OnUpdate(() =>
    {
        // Update the target position during animation
        var backuptar = target.transform.position;
        if(backuptar != null) targetPos = target.transform.position;
    }).OnComplete(() =>
    {
        projectilePool.Despawn(projectile);
        backupTarget.GetComponent<IDamageable>().TakeDamage(AbilityPower + backupAddDamage);
    });
}
}