using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class DrivePath
{
    [SerializeField, ReadOnly] private Vector3 _start;
    [SerializeField, ReadOnly] private Vector3 _target;

    public Vector3 Start => _start;
    public Vector3 Target => _target;

    public DrivePath(Vector3 start, Vector3 target)
    {
        _start = start;
        _target = target;
    }
}
