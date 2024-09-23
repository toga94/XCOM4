using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TraitEffectManager : Singleton<TraitEffectManager>
{
    public Dictionary<TraitType, int> GetTraitsStacks(List<Unit> units)
    {
        Dictionary<TraitType, int> traitStack = new Dictionary<TraitType, int>();
        HashSet<string> uniqueUnitNames = new HashSet<string>();
        foreach (Unit unit in units)
        {
            if (uniqueUnitNames.Add(unit.GetUnitName))
            {
                foreach (TraitType trait in unit.traits)
                {
                    if (traitStack.ContainsKey(trait))
                    {
                        traitStack[trait]++;
                    }
                    else
                    {
                        traitStack.Add(trait, 1);
                    }
                }
            }
        }
        return traitStack;
    }
    public List<TraitData.TraitEffect> GetActiveTraitEffects(List<TraitData> traitDataList)
    {
        List<TraitData.TraitEffect> activeEffects = new List<TraitData.TraitEffect>();

        foreach (var traitData in traitDataList)
        {
            int traitLevel = traitData.traitEffectsLevel.Length;
            var traitsUI = TraitsUI.Instance;
            int countGrid = traitsUI.units.Count;

            for (int i = traitLevel - 1; i >= 0; i--)
            {
                var traitCount = traitsUI.traitCounts;
                int currentTypeCount = 0;

                if (countGrid > 0 && traitCount.ContainsKey(traitData.traitType))
                {
                    currentTypeCount = traitCount[traitData.traitType];
                }

                if (currentTypeCount >= traitData.traitEffectsLevel[i])
                {
                    if (i < traitData.traitEffects.Length)
                    {
                        activeEffects.Add(traitData.traitEffects[i]);
                    }
                }
            }
        }

        return activeEffects;
    }


    private void OnAnyUnitMovedGridPosition(object sender, InventoryGrid.OnAnyUnitMovedInventoryPositionEventArgs e)
    {
        Invoke(nameof(UpdateActiveTraitEffects), 0.001f);
    }
    private void OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        Invoke(nameof(UpdateActiveTraitEffects), 0.001f);
    }
    private void Start(){
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += OnAnyUnitMovedGridPosition;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += OnAnyUnitMovedGridPosition;
    }
    public List<TraitData.TraitEffect> ActiveTraitEffects;
    public List<TraitData.TraitEffect> ActiveTraitEffectsWithAura;
    public TraitData.TraitEffect SummarizedActiveEffectsWithAura;

    private void UpdateActiveTraitEffects() {
       // ActiveTraitEffects = GetActiveTraitEffects(TraitDataManager.Instance.traitDataList);
      //  ActiveTraitEffectsWithAura = ActiveTraitEffects.Where(effect => effect.hasAura).ToList();
        //SummarizedActiveEffectsWithAura = SummarizeActiveEffects(ActiveTraitEffectsWithAura);
    }

    private TraitData.TraitEffect SummarizeActiveEffects(List<TraitData.TraitEffect> activeTraitEffects)
    {
        TraitData.TraitEffect summaryEffect = new TraitData.TraitEffect();


        foreach (var effect in activeTraitEffects)
        {
            if (effect.activationTiming == TraitData.ActivationTiming.OnUpdate)
            {
                summaryEffect.criticalDamagePercent += effect.criticalDamagePercent;
                summaryEffect.attackSpeed += effect.attackSpeed;
                summaryEffect.skillDamage += effect.skillDamage;
                summaryEffect.physicalDamage += effect.physicalDamage;
                summaryEffect.healthPoints += effect.healthPoints;
                summaryEffect.healthRegen += effect.healthRegen;
                summaryEffect.manaPoints += effect.manaPoints;
                summaryEffect.manaRegen += effect.manaRegen;
                summaryEffect.evasion += effect.evasion;
                summaryEffect.damageShield += effect.damageShield;
                summaryEffect.vampirism += effect.vampirism;
            }
        }

        return summaryEffect;
    }

    public List<TraitData.TraitEffect> GetAllActiveTraitEffects(TraitType traitType)
    {
        List<TraitData.TraitEffect> activeEffects = new List<TraitData.TraitEffect>();
        List<TraitData> traitDataList = TraitDataManager.Instance.traitDataList;
        activeEffects.AddRange(GetActiveTraitEffectsWithNonTraitType(traitDataList));
        activeEffects.AddRange(GetActiveTraitEffectsWithSameTraitType(traitDataList, traitType));
        return activeEffects;
    }
    public List<TraitData.TraitEffect> GetActiveTraitEffectsWithNonSameTraitType(List<TraitData> traitDataList, TraitType traitType)
    {
        List<TraitData.TraitEffect> activeEffects = new List<TraitData.TraitEffect>();
        var traitsUI = TraitsUI.Instance;
        foreach (var traitData in traitDataList)
        {
            int traitLevel = traitData.traitEffectsLevel.Length;

            int countGrid = traitsUI.units.Count;

            for (int i = traitLevel - 1; i >= 0; i--)
            {
                var traitCount = traitsUI.traitCounts;
                int currentTypeCount = 0;

                if (countGrid > 0 && !traitCount.ContainsKey(traitData.traitType))
                {
                    currentTypeCount = traitCount[traitData.traitType];
                }

                if (currentTypeCount >= traitData.traitEffectsLevel[i])
                {
                    if (i < traitData.traitEffects.Length)
                    {
                        if (traitData.traitType == traitType)
                        {
                            activeEffects.Add(traitData.traitEffects[i]);
                        }

                    }
                }
            }
        }

        return activeEffects;
    }
    public List<TraitData.TraitEffect> GetActiveTraitEffectsWithNonTraitType(List<TraitData> traitDataList)
    {
        List<TraitData.TraitEffect> activeEffects = new List<TraitData.TraitEffect>();
        var traitsUI = TraitsUI.Instance;
        foreach (var traitData in traitDataList)
        {
            int traitLevel = traitData.traitEffectsLevel.Length;

            int countGrid = traitsUI.units.Count;

            for (int i = traitLevel - 1; i >= 0; i--)
            {
                var traitCount = traitsUI.traitCounts;
                int currentTypeCount = 0;

                if (countGrid > 0 && traitCount.ContainsKey(traitData.traitType))
                {
                    currentTypeCount = traitCount[traitData.traitType];
                }

                if (currentTypeCount >= traitData.traitEffectsLevel[i] && !traitData.traitEffects[i].effectOnlyForTraitOwners)
                {
                    if (i < traitData.traitEffects.Length)
                    {
                            activeEffects.Add(traitData.traitEffects[i]);
                    }
                }
            }
        }

        return activeEffects;
    }
    public List<TraitData.TraitEffect> GetActiveTraitEffectsWithSameTraitType(List<TraitData> traitDataList, TraitType traitType)
    {
        List<TraitData.TraitEffect> activeEffects = new List<TraitData.TraitEffect>();
        var traitsUI = TraitsUI.Instance;
        foreach (var traitData in traitDataList)
        {
            int traitLevel = traitData.traitEffectsLevel.Length;
          
            int countGrid = traitsUI.units.Count;

            for (int i = traitLevel - 1; i >= 0; i--)
            {
                var traitCount = traitsUI.traitCounts;
                int currentTypeCount = 0;

                if (countGrid > 0 && traitCount.ContainsKey(traitData.traitType))
                {
                    currentTypeCount = traitCount[traitData.traitType];
                }

                if (currentTypeCount >= traitData.traitEffectsLevel[i] && traitData.traitEffects[i].effectOnlyForTraitOwners)
                {
                    if (i < traitData.traitEffects.Length)
                    {
                        if (traitData.traitType == traitType) {
                            activeEffects.Add(traitData.traitEffects[i]);
                        }

                    }
                }
            }
        }

        return activeEffects;
    }
    private TraitData.TraitEffect SummarizeActiveEffectsWithSameTraitOwners(TraitType traitType)
    {
        var activeTraitEffects = GetActiveTraitEffectsWithSameTraitType(TraitDataManager.Instance.traitDataList, traitType);
        TraitData.TraitEffect summaryEffect = new TraitData.TraitEffect();


        foreach (var effect in activeTraitEffects)
        {
            if (effect.activationTiming == TraitData.ActivationTiming.OnUpdate)
            {
                summaryEffect.criticalDamagePercent += effect.criticalDamagePercent;
                summaryEffect.attackSpeed += effect.attackSpeed;
                summaryEffect.skillDamage += effect.skillDamage;
                summaryEffect.physicalDamage += effect.physicalDamage;
                summaryEffect.healthPoints += effect.healthPoints;
                summaryEffect.healthRegen += effect.healthRegen;
                summaryEffect.manaPoints += effect.manaPoints;
                summaryEffect.manaRegen += effect.manaRegen;
                summaryEffect.evasion += effect.evasion;
                summaryEffect.damageShield += effect.damageShield;
                summaryEffect.vampirism += effect.vampirism;
            }
        }

        return summaryEffect;
    }


    public List<TraitData.TraitEffect> TraitEffectsForUnit(Unit unit)
    {
        List<TraitData.TraitEffect> traitEffects = new List<TraitData.TraitEffect>();

        List<TraitType> traitTypes = unit.traits;

        foreach (var traitType in traitTypes)
        {
            var newEffects = GetAllActiveTraitEffects(traitType).Except(traitEffects).ToList();
            traitEffects.AddRange(newEffects);
        }

        return traitEffects;
    }

}
