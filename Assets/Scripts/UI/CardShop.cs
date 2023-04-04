using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;
using System.Linq;

public class CardShop : MonoBehaviour
{

    public event EventHandler<UnitObject[]> onItemAdded;

    public static CardShop Instance { get; private set; }

    [SerializeField] private UnitObject[] allUnitObjects;
    [SerializeField] private UnitObject[] unitInShops;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one CardShop! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public void RandomSelect5ItemForShop()
    {
        GameManager gm = GameManager.Instance;
        int cost = 2;
        if (gm.CanIBuy(cost))
        {
            List<UnitObject> allUnitsList = new List<UnitObject>(allUnitObjects);
            UnitObject[] selectedUnits = RandomPick(allUnitObjects, 5).ToArray();
            gm.SubtractGold(cost);
            onItemAdded?.Invoke(this, selectedUnits);
        }

    }

    private static List<T> RandomPick<T>(IList<T> list, int numItemsToSelect) where T : UnitObject
    {
        List<T> selectedItems = new List<T>();

        for (int i = 0; i < numItemsToSelect; i++)
        {
            // Calculate the cumulative probability of selecting each item
            var cumulativeProbabilities = new Dictionary<T, float>();
            float cumulativeProbability = 0;
            foreach (var item in list)
            {
                float itemProbability = GameManager.Instance.GetItemProbability(item);
                cumulativeProbability += itemProbability;
                cumulativeProbabilities[item] = cumulativeProbability;
            }

            // Select a random item based on the specified probabilities
            float randomValue = Random.Range(0, cumulativeProbability);
            T selectedItem = cumulativeProbabilities.First(pair => pair.Value >= randomValue).Key;

            selectedItems.Add(selectedItem);
        }

        return selectedItems;
    }

}
