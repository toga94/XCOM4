using UnityEngine;

[CreateAssetMenu(menuName = "Game Data/Item")]
public class Item : ScriptableObject
{
    public string name;
    public string description;
    public ItemEffect ItemEffect;
}
