using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Ability")]
public class Ability : ScriptableObject
{
    public string name;
    public int manaCost;
    public int damage;
    public float cooldown;
}