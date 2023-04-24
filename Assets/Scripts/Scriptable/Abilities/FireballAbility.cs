using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAbility : IAbility
{
    [SerializeField] private string abilityName;
    [SerializeField] private Sprite icon;
    [SerializeField] private float abilityPower;
    [SerializeField] private int manaCost;
    [SerializeField] private bool isOffensive;
    [SerializeField] private GameObject prefab;

    public GameObject Prefab => prefab;

    string IAbility.AbilityName => abilityName;
    Sprite IAbility.Icon => icon;

    float IAbility.AbilityPowerAmount()
    {
        return abilityPower;
    }

    void IAbility.Activate()
    {
        //
    }

    int IAbility.GetManaCost()
    {
        return manaCost;
    }

    bool IAbility.IsOffensive()
    {
        return isOffensive;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
