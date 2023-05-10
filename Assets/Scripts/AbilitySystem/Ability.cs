using UnityEngine;

public abstract class Ability : MonoBehaviour
{
    public Sprite Icon;
    public string AbilityName;
    public string Description;
    public float Cooldown;
    public float CastTime;
    public float AbilityPower;
    public float ManaCost;
    public bool IsOffensive;
    public AbilityType abilityType;

    public abstract void Cast(GameObject target, float additionalDamage);
    public bool OnCooldown()
    {
        // Check if ability is on cooldown
        return false;
    }
}