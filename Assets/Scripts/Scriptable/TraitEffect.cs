using UnityEngine;

public partial class TraitData
{
    [System.Serializable]
    public class TraitEffect
    {
        public string effectDescription;
        [Header("Aura Settings")]
        public bool hasAura;
        public bool effectOnlyForTraitOwners;
        [Header("Addional Parameters Settings")]
        public float criticalDamagePercent;
        public float attackSpeed;
        public float skillDamage;
        public float physicalDamage;
        public float healthPoints;
        public float healthRegen;
        public float manaPoints;
        public float manaRegen;
        public float evasion;
        public float damageShield;
        public float vampirism;

        [Header("Conditions Settings")]
   

        public ActivationTiming activationTiming;
    
        public float timer;
        public float conditionDamage;
        public float conditionHealthPoints;
        public float conditionManaPoints;
    }
    public enum ActivationTiming
    {
        None,
        OnUpdate,
        AtStart
    }
}