using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/ItemEffect")]
public class ItemEffect
{
    public enum EffectType { Health, AttackPower, Mana };
    public EffectType type;
    public int value;
}