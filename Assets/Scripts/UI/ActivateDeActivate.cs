using UnityEngine;

public class ActivateDeActivate : MonoBehaviour
{
    public void OnClick(GameObject gameObject) {
        gameObject.SetActive(!gameObject.active);
    }
}
