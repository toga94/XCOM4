using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TraitsUI : MonoBehaviour
{
    public List<Unit> units;
    public GameObject traitPrefab;
    public Transform traitList;

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
      Invoke("UpdateTraits", 0.1f);
    }
    private void OnAnyUnitMovedGridPosition(object sender, LevelGrid.OnAnyUnitMovedGridPositionEventArgs e)
    {
        Invoke("UpdateTraits", 0.1f);
    }

    void UpdateList(object sender, EventArgs e)
    {
        Invoke("UpdateTraits", 0.1f);
    }

    private void UpdateTraits()
    {
        units = GameManager.Instance.GetAllUnitsOnGrid;
        foreach (Transform child in traitList)
        {
            Destroy(child.gameObject);
        }

        Dictionary<TraitType, int> traitCounts = GetTraitsStacks(units);

        foreach (KeyValuePair<TraitType, int> kvp in traitCounts)
        {
            GameObject traitObject = Instantiate(traitPrefab, traitList);

            Image traitIcon = traitObject.transform.Find("TraitIcon").GetComponent<Image>();
            traitIcon.sprite = GetTraitSprite(kvp.Key);

            Text traitName = traitObject.transform.Find("TraitName").GetComponent<Text>();
            traitName.text = kvp.Key.ToString();

            Text traitStack = traitObject.transform.Find("TraitCount").GetComponent<Text>();
            traitStack.text = $"{kvp.Value} / {GetTraitMaxStack(kvp.Key)}";
        }
    }

    private Sprite GetTraitSprite(TraitType trait)
    {
        TraitData traitdata = TraitDataManager.Instance.GetTraitData(trait);

        return traitdata.traitSprite;
    }

    private int GetTraitMaxStack(TraitType trait)
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