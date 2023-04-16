using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName;
    [SerializeField] private int unitLevel;
    public int type; // Type of unit (e.g. archer, knight, mage)
    public int tier; // Tier of unit (e.g. 1-star, 2-star, 3-star)
    public bool OnGrid { get; set; }
    public float MaxHealth { get; set; }

    public string GetUnitNameWithLevel => $"{unitName}{unitLevel}";
    public string GetUnitName => $"{unitName}";
    public int GetUnitLevel => unitLevel;
    public Vector3 UnitPosition;
    public GridPosition UnitGridPosition;
    public List<TraitType> traits;
    public CharState charState;

    [SerializeField]private UnitObject unitObject;
    private Object dropUnitFX;

    public UnitObject GetUnitObject => unitObject;

    private Animator animator;

    private GameStateSystem stateSystem;


    public void UpgradeLevel()
    {
        unitLevel++;
        gameObject.name = GetUnitNameWithLevel;
        if (animator == null) animator = GetComponent<Animator>();
        animator?.Play("level_up");
    }

    public void TeleportToPosition(Vector3 targetPosition, GridPosition unitGridPosition)
    {
        UnitGridPosition = unitGridPosition;
        transform.position = targetPosition;
        UnitPosition = targetPosition;
        Instantiate(dropUnitFX, targetPosition + Vector3.up / 2, Quaternion.identity);

    }
    private void Awake()
    {
        traits = unitObject.traits;
        dropUnitFX = Resources.Load("DropUnitFX");
    }
    private void Start()
    {
        animator = GetComponent<Animator>();
        stateSystem = GameStateSystem.Instance;
    }
    public void UpdatePos(TransformData data)
    {
        if (!OnGrid && GameStateSystem.Instance.GetCurrentState() is ChampionSelectionState) return;
            transform.SetPositionAndRotation(data.position, data.rotation);
    }
}
