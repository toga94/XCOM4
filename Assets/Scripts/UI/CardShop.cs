using UnityEngine;
using System.Collections.Generic;
using System;

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
            Debug.LogError("There's more than one InventoryGrid! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }



    public void RandomSelect5ItemForShop()
    {

        List<UnitObject> allUnitsList = new List<UnitObject>(allUnitObjects);


        Shuffle(allUnitsList);


        UnitObject[] selectedUnits = allUnitsList.GetRange(0, 5).ToArray();


        unitInShops = selectedUnits;

        onItemAdded?.Invoke(this, unitInShops);
    }


    private void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }





 
}
