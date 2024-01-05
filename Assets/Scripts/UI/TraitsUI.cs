using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;

public class TraitsUI : Singleton<TraitsUI>
{
    public List<Unit> units;
    public GameObject traitPrefab;
    public Transform traitList;

    [SerializeField] private LeanGameObjectPool pool;

    private void Start()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += OnAnyUnitMovedGridPosition;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition += OnAnyUnitMovedGridPosition;
    }

    private void OnAnyUnitMovedGridPosition(object sender, InventoryGrid.OnAnyUnitMovedInventoryPositionEventArgs e)
    {
        Invoke(nameof(UpdateTraits), 0.001f);
    }
    private void OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        Invoke(nameof(UpdateTraits), 0.001f);
    }

    public Dictionary<TraitType, int> traitCounts;
    private void UpdateTraits()
    {
        units = GameManager.Instance.GetAllUnitsOnGrid;
        pool.DespawnAll();

        traitCounts = GetTraitsStacks(units);

        foreach (KeyValuePair<TraitType, int> kvp in traitCounts)
        {
            float stackRatio = GetTraitMaxStack(kvp.Key) - kvp.Value;

            GameObject traitObject = pool.Spawn(traitList);

            var traitUITrigger = traitObject.GetComponent<TraitTooltipTrigger>();
            traitUITrigger.traitData = TraitDataManager.Instance.FetchTraitData(kvp.Key);

            Image traitIcon = traitObject.transform.Find("TraitIcon").GetComponent<Image>();
            traitIcon.sprite = GetTraitSprite(kvp.Key);

            Text traitName = traitObject.transform.Find("TraitName").GetComponent<Text>();
            traitName.text = kvp.Key.ToString();

            Text traitStack = traitObject.transform.Find("TraitCount").GetComponent<Text>();
            traitStack.text = $"{kvp.Value} / {GetTraitMaxStack(kvp.Key)}";

            Image traitIconBg = traitObject.transform.Find("TraitIconBg").GetComponent<Image>();
            traitIconBg.color = GetTraitSpriteColor(kvp.Key, kvp.Value);
            traitIconBg.color -= new Color(0f, 0f, 0f, 0.5f);
        }


    }

    private Sprite GetTraitSprite(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.FetchTraitData(trait);
        return traitdata.traitSprite;
    }

    //public Color GetTraitSpriteColor(TraitType trait, int level)
    //{
    //    TraitData traitdata = TraitDataManager.Instance.FetchTraitData(trait);
    //    int maxStack = GetTraitMaxStack(trait);

    //    int colorIndex = Mathf.Min(traitdata.traitEffectsLevel.Length - 1, level - 1);

    //    Color[] traitColors = new Color[]
    //    {
    //        new Color(0f, 0f, 0f), // level 1 color
    //        new Color(0f, 0f, 1f), // level 2 color
    //        new Color(0f, 1f, 0f), // level 3 color
    //        new Color(1f, 0.8f, 0f), // level 4 color
    //        new Color(1f, 0f, 0f) // level 5 color
    //    };

    //    return traitColors[colorIndex];
    //}
    private Color GetTraitSpriteColor(TraitType trait, int level)
    {
        TraitData traitdata = TraitDataManager.Instance.FetchTraitData(trait);
        int maxStack = GetTraitMaxStack(trait);

        Color[] traitColors = new Color[] {
        new Color(0f, 0f, 0f), // level 1 color
        new Color(0f, 0f, 1f), // level 2 color
        new Color(0f, 1f, 0f), // level 3 color
        new Color(1f, 0.8f, 0f), // level 4 color
        new Color(1f, 0f, 0f) // level 5 color
        };

        int colorIndex = 0;
        var traitEffectLevel = traitdata.traitEffectsLevel;
        var allTraitEffects = traitdata.traitEffectsLevel;
        if (traitEffectLevel.Length > 0 && allTraitEffects[0] <= level) colorIndex = 4;
        else if (traitEffectLevel.Length > 1 && allTraitEffects[1] <= level) colorIndex = 3;
        else if (traitEffectLevel.Length > 2 && allTraitEffects[2] <= level) colorIndex = 2;
        else if (traitEffectLevel.Length > 3 && allTraitEffects[3] <= level) colorIndex = 1;
        else if (traitEffectLevel.Length > 4 && allTraitEffects[4] <= level) colorIndex = 0;
        return traitColors[colorIndex];
    }
    private int GetTraitMaxStack(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.FetchTraitData(trait);
        return traitdata.traitEffectsLevel[0];
    }

    public Dictionary<TraitType, int> GetTraitsStacks(List<Unit> units)
    {
        Dictionary<TraitType, int> traitStack = new Dictionary<TraitType, int>();

        foreach (Unit unit in units)
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

        return traitStack;
    }
}
