using NaughtyAttributes;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshWalkableController : Singleton<NavMeshWalkableController>
{
    [SerializeField] private List<Transform> _pathTargetsParents;
    [SerializeField] private Transform _homeTargetsParent;

    [SerializeField, ReadOnly] private List<Vector3> _pathTargets = new();
    [SerializeField, ReadOnly] private List<Vector3> _homeTargets = new();

    [Button]
    public void GetTargets()
    {
        _pathTargets.Clear();
        _homeTargets.Clear();

        foreach (Transform pathParent in _pathTargetsParents)
        {
            foreach (Transform p in pathParent)
            {
                _pathTargets.Add(p.position);
            }
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
        foreach (Transform t in _pathTargetsParents)
        {
            Destroy(t.gameObject);
        }

        Destroy(_homeTargetsParent.gameObject);
    }
}
