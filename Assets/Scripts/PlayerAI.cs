using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Player
{
    public string PlayerName;
    public int playerLevel;
    public List<Unit> gridUnits;
    public List<Vector3> gridUnitsPositions;
    public List<Unit> inventoryUnits;
    public List<Vector3> inventoryUnitsPosition;
}

public class PlayerAI : MonoBehaviour
{

}
