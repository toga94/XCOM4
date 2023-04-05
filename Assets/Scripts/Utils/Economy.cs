using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Economy : MonoBehaviour
{
    public static int GetUnitCost(int unitLevel, RareOptions rareOptions)
    {
        int rarityCost = (int)rareOptions + 1;
        int result = ((int)Mathf.Pow(3, unitLevel)) * rarityCost;
        return result;
    }
}
