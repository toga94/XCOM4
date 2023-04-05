using System;
using UnityEngine;

public static class Economy
{
    public static int gold = 80;
    private static Action<int> goldChanged;

    public static Action<int> OnGoldChanged
    {
        get { return goldChanged; }
        set
        {
            Debug.Log("GoldChanged event set to: " + (value != null));
            goldChanged = value;
        }
    }


    public static int GetUnitCost(int unitLevel, RareOptions rareOptions)
    {
        int rarityCost = (int)rareOptions + 1;
        int result = ((int)Mathf.Pow(3, unitLevel)) * rarityCost;
        return result;
    }


    public static bool BuyUnit(Unit unit)
    {
        int result = GetUnitCost(unit.GetUnitLevel, unit.GetUnitObject.rareOptions);
        if (CanIBuy(result))
        {
            SubtractGold(result);
            return true;
        }
        else
        {
            return false;
        }
    }
    public static void SellUnit(Unit unit)
    {
        int result = GetUnitCost(unit.GetUnitLevel, unit.GetUnitObject.rareOptions);
        AddGold(result);
    }
    public static float GetItemProbability(UnitObject item)
    {
        switch (item.rareOptions)
        {
            case RareOptions.Common:
                return 0.4f;
            case RareOptions.Uncommon:
                return 0.3f;
            case RareOptions.Rare:
                return 0.2f;
            case RareOptions.Epic:
                return 0.075f;
            case RareOptions.Legendary:
                return 0.025f;
            default:
                return 0f;
        }
    }

    public static void AddGold(int goldAmount)
    {
        gold += goldAmount;
        OnGoldChanged.Invoke(gold);
    }

    public static void SubtractGold(int goldAmount)
    {
        gold -= goldAmount;
        OnGoldChanged.Invoke(gold);
    }
    public static bool CanIBuy(int amount)
    {
        return amount <= gold;
    }
    public static int GetGold()
    {
        OnGoldChanged?.Invoke(gold);
        return gold;
    }
}
