using UnityEngine;

[CreateAssetMenu(fileName = "TraitData", menuName = "Trait Data")]
public partial class TraitData : ScriptableObject
{
    public TraitType traitType;
    public string traitName;
    public string traitDescription;
    public int[] traitEffectsLevel;
    public Sprite traitSprite;


    public TraitEffect[] traitEffects;

    public void Use() {

    }

}