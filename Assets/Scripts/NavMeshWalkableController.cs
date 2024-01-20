using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshWalkableController : Singleton<NavMeshWalkableController>
{
    [SerializeField] private Transform _pathTargetsParent;
    [SerializeField] private Transform _homeTargetsParent;

    [SerializeField, ReadOnly] private List<Vector3> _pathTargets = new();
    [SerializeField, ReadOnly] private List<Vector3> _homeTargets = new();

    [Button]
    public void GetTargets()
    {
        _pathTargets.Clear();
        _homeTargets.Clear();

        foreach (Transform t in _pathTargetsParent)
        {
            _pathTargets.Add(t.position);
        }

        foreach (Transform t in _homeTargetsParent)
        {
            _homeTargets.Add(t.position);
        }
    }

    public Vector3 GetRandomPathTarget()
    {
        return _pathTargets[Random.Range(0, _pathTargets.Count)];
    }

    public Vector3 GetRandomHomeTarget()
    {
        return _homeTargets[Random.Range(0, _homeTargets.Count)];
    }

    private void Awake()
    {
        Destroy(_pathTargetsParent.gameObject);
        Destroy(_homeTargetsParent.gameObject);
    }
}
