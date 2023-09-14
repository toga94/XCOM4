using System;
using UnityEngine;

public struct Economy
{
    public static int gold = 20;
    public static int xpCost = 4;
    public static int Level { get; set; } = 1;
    public static int Exp { get; set; }
    public static int Health { get; set; } = 100;

    public static Action<int> OnGoldChanged;
    public static Action<int> OnExperienceChanged;
    public static Action<int> OnLevelChanged;
    public static Action<int> OnHealthChanged;
    public const int MIN_GOLD = 1;

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
        OnGoldChanged?.Invoke(gold);
    }

    public static void SubtractGold(int goldAmount)
    {
        gold -= goldAmount;
        OnGoldChanged?.Invoke(gold);
    }
    public static void SubtractHealth(int healthAmount)
    {
        Health -= healthAmount;
        OnHealthChanged?.Invoke(Health);
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



    public static void GainExperience(int amount)
    {
        Exp += amount;
        if (Exp >= GetExperienceNeededForNextLevel())
        {
            LevelUp();
        }
        OnExperienceChanged?.Invoke(Exp);

    }
    private static void LevelUp()
    {
        Exp -= GetExperienceNeededForNextLevel();
        Level++;
        OnLevelChanged?.Invoke(Level);
    }

    public static int GetExperienceNeededForNextLevel()
    {
        int experienceNeeded = Level + 4;
        return experienceNeeded;
    }
}
