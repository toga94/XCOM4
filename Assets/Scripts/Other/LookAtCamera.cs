using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert;
    private Transform cameraTransform;
    private Transform cachedTransform;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        cachedTransform = transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward, cameraTransform.rotation * Vector3.up);
    }
}