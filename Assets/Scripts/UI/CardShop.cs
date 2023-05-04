using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class CardShop : Singleton<CardShop>
{
    public event EventHandler<UnitObject[]> onItemsChanged;
    [SerializeField] private UnitObject[] allUnitObjects;
    [SerializeField] private UnitObject[] unitInShops;
    [SerializeField] private GameObject shopMenuUI;
    [SerializeField] private GameObject shopMenuRefreshUI;
 

    public void OpenShopMenu()
    {
        shopMenuUI.SetActive(true);
    }

    public void RandomSelect5ItemForShop()
    {
        int cost = 2;
        Refresh(cost);
    }
    public void RandomSelect5ItemForShopFree()
    {
        int cost = 0;
        Refresh(cost);
    }
    private void Refresh(int cost)
    {
        if (Economy.CanIBuy(cost))
        {
            List<UnitObject> allUnitsList = new List<UnitObject>(allUnitObjects);
            UnitObject[] selectedUnits = RandomPick(allUnitObjects, 5).ToArray();
            Economy.SubtractGold(cost);
            onItemsChanged?.Invoke(this, selectedUnits);
        }
    }


    private static List<T> RandomPick<T>(IList<T> list, int numItemsToSelect) where T : UnitObject
    {
        List<T> selectedItems = new List<T>();
        Dictionary<T, float> cumulativeProbabilities = new Dictionary<T, float>();
        float cumulativeProbability = 0;

        for (int i = 0; i < list.Count; i++)
        {
            float itemProbability = Economy.GetItemProbability(list[i]);
            cumulativeProbability += itemProbability;
            cumulativeProbabilities[list[i]] = cumulativeProbability;
        }

        for (int i = 0; i < numItemsToSelect; i++)
        {
            // Select a random item based on the pre-calculated probabilities
            float randomValue = Random.Range(0, cumulativeProbability);
            T selectedItem = cumulativeProbabilities.First(pair => pair.Value >= randomValue).Key;

            selectedItems.Add(selectedItem);
        }

        return selectedItems;
    }

}
