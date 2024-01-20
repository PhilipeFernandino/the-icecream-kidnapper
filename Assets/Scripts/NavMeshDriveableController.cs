using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavMeshDriveableController : Singleton<NavMeshDriveableController>
{
    [SerializeField] private Transform _pathsParent;
    [SerializeField, ReadOnly] private List<DrivePath> _drivePaths = new();

    [Button]
    private void GetPaths()
    {
        foreach (Transform t in _pathsParent.transform)
        {
            var a = t.GetChild(0);
            var b = t.GetChild(1);

            if (a == null || b == null)
            {
                Debug.LogError($"{GetType()} - A or B null in {t}");
                continue;
            }

            var drivePath = new DrivePath(a.position, b.position);
            _drivePaths.Add(drivePath);
        }
    }

    public DrivePath GetRandomPath()
    {
        return _drivePaths[Random.Range(0, _drivePaths.Count)];
    }
}
