using UnityEngine;

public interface IAbility
{
    string AbilityName { get; }
    Sprite Icon { get; }
    GameObject Prefab { get; }
    void Activate();
    int GetManaCost();
    float AbilityPowerAmount();
    bool IsOffensive();
}