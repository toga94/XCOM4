using UnityEngine;

public class SetupUI : MonoBehaviour
{
    private void Awake()
    {
        GameObject unitCanvas = (GameObject)Instantiate(Resources.Load("UnitWorldUI"), transform);
        unitCanvas.transform.localPosition = new Vector3(0, 1, 0) * 3;
        // var healthSystem = gameObject.AddComponent<HealthSystem>();
    }
}
