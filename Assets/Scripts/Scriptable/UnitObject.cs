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
    public List<ClassData> unitClassDataList;
    public GameObject Prefab;

    public enum RareOptions
    {
        OneStarUnit,
        TwoStarUnit,
        ThreeStarUnit,
        LegendaryUnit
    }
    public RareOptions rareOptions = RareOptions.OneStarUnit;

}
