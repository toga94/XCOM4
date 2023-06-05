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
       MeleeCast(target, additionalDamage);
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
    private GameObject backupTarget;
    private float backupAddDamage;
    private void MeleeCast(GameObject target, float additionalDamage)
    {
        if (target == null)
        {
            return;
        }
        backupTarget = target;
        backupAddDamage = additionalDamage;
        Vector3 targetPos = target.transform.position;
        animator = GetComponent<Animator>();
        animator.Play(base.AbilityName);
        Vector3 direction = (targetPos - transform.position).normalized;
        if (backupTarget == null)
        {
            return;
        }
        else {
            backupTarget.GetComponent<IDamageable>().TakeDamage(AbilityPower + backupAddDamage);
        }

    }
}