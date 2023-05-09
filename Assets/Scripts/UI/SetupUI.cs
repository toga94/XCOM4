using UnityEngine;
using Lean.Pool;
public class SetupUI : MonoBehaviour
{
    [SerializeField] private bool is3D;

    private void Awake()
    {
        if (is3D)
        {
            GameObject unitCanvas = (GameObject)Instantiate(Resources.Load("UnitWorldUI"), transform);
            unitCanvas.transform.localPosition = new Vector3(0, 1, 0) * 3;
            // var healthSystem = gameObject.AddComponent<HealthSystem>();
        }
    }
}
