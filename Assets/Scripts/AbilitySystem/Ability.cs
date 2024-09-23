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

    private float lastCastTime;
    public abstract void Cast(GameObject target, float additionalDamage);
    public bool OnCooldown()
    {
        float timeSinceLastCast = Time.time - lastCastTime;

        return timeSinceLastCast < Cooldown;
    }

    protected void AbilityCast()
    {

        lastCastTime = Time.time;
    }

    public virtual void CastWithCooldown(GameObject target, float additionalDamage)
    {
        if (!OnCooldown())
        {
            Cast(target, additionalDamage);
            AbilityCast(); 
        }
        else
        {
            Debug.Log("Ability is on cooldown!");
        }
    }

}