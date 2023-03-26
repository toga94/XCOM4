using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Game Data/UnitObject")]
public class UnitObject : ScriptableObject
{
    public string unitName;
    public Sprite unitImage;
    public int health;
    public int attackPower;
    public int speed;
    public int mana;
    public Ability ability;
    public List<TraitType> traits;
    public GameObject Prefab;
    public RareOptions rareOptions = RareOptions.OneStarUnit;
}
