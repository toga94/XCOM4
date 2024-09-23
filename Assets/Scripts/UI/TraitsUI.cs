using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using MoreMountains.Tools;
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


    private Color GetTraitSpriteColor(TraitType trait, int level)
    {
        TraitData traitData = TraitDataManager.Instance.FetchTraitData(trait);
        int maxStack = GetTraitMaxStack(trait);

        Color[] traitColors = new Color[]
        {
            MMColors.Black,          // level 1 color 
            MMColors.Blue,           // level 2 color
            MMColors.DarkSeaGreen,   // level 3 color
            MMColors.BlueViolet,      // level 4 color
            MMColors.BestRed              // level 5 color
        };

        int colorIndex = 0;
        var allTraitEffects = traitData.traitEffectsLevel;
        for (int i = 0; i < allTraitEffects.Length; i++)
        {
            if (allTraitEffects[i] <= level)
            {
                colorIndex = 4 - i;
                break;
            }
        }

        return traitColors[colorIndex];
    }

    public TraitData.TraitEffect GetActiveTraitEffects()
    {
        // Implement activeTraits here
        // Add your logic to determine and return active traits
        // This method should return TraitData.TraitEffect or a suitable type based on your requirements
        return null; // Placeholder, replace with the actual implementation
    }
    private int GetTraitMaxStack(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.FetchTraitData(trait);
        return traitdata.traitEffectsLevel[0];
    }

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
}
