using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/ClassData")]
public class ClassData : ScriptableObject
{
    public string name;
    public int health;
    public int attackPower;
    public int speed;
    public int mana;
}
