using System;
using System.Collections;
using UnityEngine;

public class KidSpawner : Singleton<KidSpawner>
{
    [SerializeField] private KidController _kidPrefab;
    [SerializeField] private int _kidMinHoursOnStreet;
    [SerializeField] private int _kidMaxHoursOnStreet;

    private void Awake()
    {
        Spawn();
    }

    private void Spawn()
    {
        var home = NavMeshWalkableController.Instance.GetRandomHomeTarget();
        var kid = Instantiate(_kidPrefab, home, Quaternion.identity);
        kid.Setup(home, UnityEngine.Random.Range(_kidMinHoursOnStreet, _kidMaxHoursOnStreet + 1));
    }
}
