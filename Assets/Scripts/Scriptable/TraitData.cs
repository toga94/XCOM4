using UnityEngine;

[CreateAssetMenu(fileName = "TraitData", menuName = "Trait Data")]
public class TraitData : ScriptableObject
{
    public TraitType traitType;
    public string traitName;
    public string traitDescription;
    public int[] traitEffectsLevel;
    public Sprite traitSprite;
    // add any other trait data fields here
}