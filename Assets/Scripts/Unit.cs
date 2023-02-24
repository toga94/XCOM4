using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private float moveSpeed = 4f;
    private float stoppingDistance = .1f;
    private Vector3 targetPosition;
    [SerializeField]private string unitName;
    [SerializeField] private int unitLevel;
    public bool addedtoGrid;

    private GridPosition gridPosition;

    public string GetUnitName { get => unitName + unitLevel; }

    public void Move (Vector3 targetPosition)
    {
        Vector3 offset = new Vector3(-2.5f, 0f, 2.5f);
        transform.position = targetPosition;
        Debug.Log(GetUnitName + " is moved");
    }
}
