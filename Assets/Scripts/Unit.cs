using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName;
    [SerializeField] private int unitLevel;
    public int type; // Type of unit (e.g. archer, knight, mage)
    public int tier; // Tier of unit (e.g. 1-star, 2-star, 3-star)
    public bool OnGrid { get; set; }

    public string GetUnitName => $"{unitName}{unitLevel}";
    public int GetUnitLevel => unitLevel;
    public void UpgradeLevel()
    {

            unitLevel++;

    }

    public void Move(Vector3 targetPosition) => transform.position = targetPosition;

}
