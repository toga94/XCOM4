using System;
using UnityEngine;

public struct EconomyManager
{
    public static int gold = 200;
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
        return ((int)Mathf.Pow(3, unitLevel)) * rarityCost;
    }

    public static bool BuyUnit(Unit unit)
    {
        int result = GetUnitCost(unit.GetUnitLevel, unit.GetUnitObject.rareOptions);
        if (CanIBuy(result))
        {
            SubtractGold(result);
            return true;
        }
        return false;
    }

    public static void SellUnit(Unit unit)
    {
        int result = GetUnitCost(unit.GetUnitLevel, unit.GetUnitObject.rareOptions);
        AddGold(result);
    }

    public static float GetItemProbability(UnitObject item)
    {
        int playerLevel = Level;
        if (playerLevel <= 2)
        {
            if (item.rareOptions == RareOptions.Common)
                return 1.0f;
            else
                return 0.0f;
        }
        else
        {
            float commonProbability = Mathf.Max(1.0f - 0.1f * (playerLevel - 2), 0.2f); 
            float uncommonProbability = Mathf.Min(0.05f * (playerLevel - 2), 0.3f); 
            float rareProbability = Mathf.Min(0.02f * (playerLevel - 2), 0.25f);
            float epicProbability = Mathf.Min(0.01f * (playerLevel - 2), 0.15f); 
            float legendaryProbability = Mathf.Min(0.005f * (playerLevel - 2), 0.1f); 

            switch (item.rareOptions)
            {
                case RareOptions.Common:
                    return commonProbability;
                case RareOptions.Uncommon:
                    return uncommonProbability;
                case RareOptions.Rare:
                    return rareProbability;
                case RareOptions.Epic:
                    return epicProbability;
                case RareOptions.Legendary:
                    return legendaryProbability;
                default:
                    return 0.0f;
            }
        }
    }


    public static void AddGold(int goldAmount)
    {
        GameManager.Instance.GetAddGoldPartice.Play();
        gold += goldAmount;
        OnGoldChanged?.Invoke(gold);
    }

    public static void SubtractGold(int goldAmount)
    {
        GameManager.Instance.GetSubGoldPartice.Play();
        gold = Mathf.Max(0, gold - goldAmount); // Prevent negative gold
        OnGoldChanged?.Invoke(gold);
    }

    public static void SubtractHealth(int healthAmount)
    {
        Health = Mathf.Max(0, Health - healthAmount); // Prevent negative health
        OnHealthChanged?.Invoke(Health);
    }

    public static bool CanIBuy(int amount)
    {
        return amount <= gold;
    }

    public static int GetGold()
    {
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
        return Level + 4;
    }
}
