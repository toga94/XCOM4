using UnityEngine;
using System.Collections.Generic;
using System;
using Random = UnityEngine.Random;

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
        List<UnitObject> allUnitsList = new List<UnitObject>(allUnitObjects);
        UnitObject[] selectedUnits = RandomPick(allUnitObjects, 5).ToArray();
        onItemAdded?.Invoke(this, selectedUnits);
    }

    private static List<T> RandomPick<T>(IList<T> list, int numItemsToSelect)
    {
        List<T> selectedItems = new List<T>();

        for (int i = 0; i < numItemsToSelect; i++)
        {
            int randomIndex = Random.Range(0, list.Count);
            T selectedItem = list[randomIndex];
            selectedItems.Add(selectedItem);
        }

        return selectedItems;
    }





}
