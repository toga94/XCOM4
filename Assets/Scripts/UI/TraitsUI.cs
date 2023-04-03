using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
public class TraitsUI : MonoBehaviour
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
    private void OnDisable()
    {
        LevelGrid.Instance.OnAnyUnitMovedGridPosition -= OnAnyUnitMovedGridPosition;
        InventoryGrid.Instance.OnAnyUnitMovedInventoryPosition -= OnAnyUnitMovedGridPosition;
    }
    private void OnAnyUnitMovedGridPosition(object sender, InventoryGrid.OnAnyUnitMovedInventoryPositionEventArgs e)
    {
        Invoke("UpdateTraits", 0.001f);
    }
    private void OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        Invoke("UpdateTraits", 0.001f);
    }

    void UpdateList(object sender, EventArgs e)
    {
        Invoke("UpdateTraits", 0.001f);
    }

    private void UpdateTraits()
    {
        units = GameManager.Instance.GetAllUnitsOnGrid;
        pool.DespawnAll();


        Dictionary<TraitType, int> traitCounts = GetTraitsStacks(units);

        var sortedTraitCounts = traitCounts.OrderBy(kvp => (GetTraitMaxStack(kvp.Key) / kvp.Value)).ThenByDescending(kvp => kvp.Value);

        foreach (KeyValuePair<TraitType, int> kvp in sortedTraitCounts)
        {
            float stackRatio = GetTraitMaxStack(kvp.Key) - kvp.Value;
            
            GameObject traitObject = pool.Spawn(traitList) ;


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
        TraitData traitdata = TraitDataManager.Instance.GetTraitData(trait);

        return traitdata.traitSprite;
    }
    private Color GetTraitSpriteColor(TraitType trait, int level)
    {
        TraitData traitdata = TraitDataManager.Instance.GetTraitData(trait);
        int maxStack = GetTraitMaxStack(trait);

        Color[] traitColors = new Color[] {
        new Color(0f, 0f, 0f), // level 1 color
        new Color(0f, 0f, 1f), // level 2 color
        new Color(0f, 1f, 0f), // level 3 color
        new Color(1f, 0.8f, 0f), // level 4 color
        new Color(1f, 0f, 0f) // level 5 color
    };
        int colorIndex = 0;

        if (traitdata.traitEffectsLevel.Length > 0 && traitdata.traitEffectsLevel[0] <= level) colorIndex = 4;
        else if (traitdata.traitEffectsLevel.Length > 1 && traitdata.traitEffectsLevel[1] <= level) colorIndex = 3;
        else if (traitdata.traitEffectsLevel.Length > 2 && traitdata.traitEffectsLevel[2] <= level) colorIndex = 2;
        else if (traitdata.traitEffectsLevel.Length > 3 && traitdata.traitEffectsLevel[3] <= level) colorIndex = 1;
        else if (traitdata.traitEffectsLevel.Length > 4 && traitdata.traitEffectsLevel[4] <= level) colorIndex = 0;
         

        return traitColors[colorIndex];
    }
    private int GetTraitMaxStack(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.GetTraitData(trait);
        //return traitdata.traitEffectsLevel[traitdata.traitEffectsLevel.Length - 1];
        return traitdata.traitEffectsLevel[0];
    }
    private int GetTraitLength(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.GetTraitData(trait);
        return traitdata.traitEffectsLevel[traitdata.traitEffectsLevel.Length - 1];
    }
    private Dictionary<TraitType, int> GetTraitsStacks(List<Unit> units)
    {
        Dictionary<TraitType, int> traitStack = new Dictionary<TraitType, int>();
        HashSet<string> unitNames = new HashSet<string>();

        foreach (Unit unit in units)
        {
            string unitName = unit.GetUnitName;

            if (!unitNames.Contains(unitName))
            {
                unitNames.Add(unitName);

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