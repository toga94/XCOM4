using DG.Tweening;
using Lean.Pool;
using System;
using System.Collections;
using UnityEngine;
public class MeleeAbility : Ability
{
    private Animator animator;
    public string AnimationName = "melee";
    [SerializeField]
    private GameObject projectilePrefab;
    private LeanGameObjectPool projectilePool;
    [SerializeField]
    private MeleeWeaponType weaponType = MeleeWeaponType.BrassKnuckles;
    public override void Cast(GameObject target, float additionalDamage)
    {
        //PoolingSystem(nameof(MeleeAbility));
        // Cast fireball spell
        if (target == null) return;
        StartCoroutine(MeleeCast(target, additionalDamage));
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
    private IEnumerator MeleeCast(GameObject target, float additionalDamage)
    {
        if (target == null)
        {
            yield break; // exit the method if target is null
        }
        backupTarget = target;
        backupAddDamage = additionalDamage;
        Vector3 targetPos = target.transform.position;
        animator.Play(base.AbilityName);
        Vector3 direction = (targetPos - transform.position).normalized;
        backupTarget.GetComponent<IDamageable>().TakeDamage(AbilityPower + backupAddDamage);
    }
}