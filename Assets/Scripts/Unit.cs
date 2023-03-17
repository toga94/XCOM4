using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private string unitName;
    [SerializeField] private int unitLevel;
    public int type; // Type of unit (e.g. archer, knight, mage)
    public int tier; // Tier of unit (e.g. 1-star, 2-star, 3-star)
    public bool OnGrid { get; set; }
    public float MaxHealth { get; set; }
    private Animator animator;
    public string GetUnitName => $"{unitName}{unitLevel}";
    public int GetUnitLevel => unitLevel;
    public Vector3 UnitPosition;
    public GridPosition UnitGridPosition;
    public enum CharState
    {
        Idle,
        Fall,
        Fight,
        MagicFight
    }
    public CharState charState;

    public void UpgradeLevel()
    {
        unitLevel++;
        gameObject.name = GetUnitName;
    }

    public void TeleportToPosition(Vector3 targetPosition, GridPosition unitGridPosition)
    {
        UnitGridPosition = unitGridPosition;
        transform.position = targetPosition;
        UnitPosition = targetPosition;
        Instantiate(Resources.Load("DropUnitFX"), targetPosition + Vector3.up/2, Quaternion.identity);

    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        AnimationStates();
    }

    private void AnimationStates()
    {
        if (charState == CharState.Fall)
        {
            animator.SetBool("fall", true);
        }
        else
        {
            animator.SetBool("fall", false);
        }
    }



}
