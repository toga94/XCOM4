using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Game Data/UnitObject")]
public class UnitObject : ScriptableObject
{
    public string unitName;
    public Sprite unitImage;
    public int health;
    public float healthRegen;
    public int defence;
    public int attackPower;
    public int speed;
    public int mana;
    public float manaRegen;
    public int attackRange = 15;
    public List<TraitType> traits;
    public GameObject Prefab;
    public RareOptions rareOptions = RareOptions.Common;
    public AttackType attackType = AttackType.Melee;

}
