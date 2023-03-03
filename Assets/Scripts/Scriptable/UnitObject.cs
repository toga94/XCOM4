using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Game Data/UnitObject")]
public class UnitObject : ScriptableObject
{
    public string name;
    public int health;
    public int attackPower;
    public int speed;
    public int mana;
    public Ability ability;
    public List<ClassData> unitClassDataList;
}
