using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName;
    [SerializeField] private int unitLevel;
    public bool OnGrid;

    public string GetUnitName { get => unitName + unitLevel; }

    public void Move(Vector3 targetPosition) => transform.position = targetPosition;
}
