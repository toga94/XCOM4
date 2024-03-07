using UnityEngine;

public partial class TraitData
{
    [System.Serializable]
    public class TraitEffect
    {
        public string effectDescription;
        [Header("Aura Settings")]
        public bool TraitAura;
        public bool EffectOnlyForTraitOwners;
        [Header("Addional Parameters Settings")]
        public float criticalDamagePercent;
        public float attackSpeed;
        public float skillDamage;
        public float physicalDamage;
        public float healthPoint;
        public float healthRegen;
        public float manaPoint;
        public float manaRegen;
        public float evasion;
        public float damageShield;

        [Header("Conditions Settings")]
        public bool oneTime;
        public bool atStart;
        public float timer;
        public float cond_Damage;
        public float cond_HealthPoint;
        public float cond_ManaPoint;
    }

}