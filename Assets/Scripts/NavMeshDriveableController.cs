using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshDriveableController : Singleton<NavMeshDriveableController>
{
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private Transform _targetsParent;
    [SerializeField] private Transform _spawn;
    [SerializeField, ReadOnly] private List<Vector3> _driveTargets = new();

    [Button]
    private void SetPaths()
    {
        _driveTargets.Clear();

        foreach (Transform t in _targetsParent.transform)
        {
            _driveTargets.Add(t.position);
        }
    }

    public DrivePath GetRandomPath()
    {
        return new DrivePath(_spawn.position, _driveTargets[Random.Range(0, _driveTargets.Count)]);
    }
}
