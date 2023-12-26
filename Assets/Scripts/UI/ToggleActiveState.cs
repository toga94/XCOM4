using UnityEngine;

public class ToggleActiveState : MonoBehaviour
{
    public void OnClick(GameObject gameObject) {
        gameObject.SetActive(!gameObject.active);
    }
}
