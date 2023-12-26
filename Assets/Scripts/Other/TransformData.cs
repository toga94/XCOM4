using UnityEngine;
[SerializeField]
public struct TransformData
{
    public Vector3 position;
    public Quaternion rotation;

    public TransformData(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}