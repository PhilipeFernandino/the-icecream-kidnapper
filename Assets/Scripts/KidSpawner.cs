using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidSpawner : Singleton<KidSpawner>
{
    [SerializeField] private List<KidController> _kidPrefabs = new();
    [SerializeField] private int _kidMinHoursOnStreet;
    [SerializeField] private int _kidMaxHoursOnStreet;
    [SerializeField] private AnimationCurve _kidSpawnPerHour;

    private void Awake()
    {
        SpawnKidsByHour();
        DaytimeSystem.Instance.HourPassed += HourPassedEventHandler;
    }

    private void HourPassedEventHandler()
    {
        SpawnKidsByHour();
    }

    private void SpawnKidsByHour()
    {
        int spawnKids = Mathf.CeilToInt(_kidSpawnPerHour.Evaluate(DaytimeSystem.Instance.CurrentDayTime.Hour));

        for (int i = 0; i < spawnKids; i++)
        {
            SpawnKid();
        }
    }

    private void SpawnKid()
    {
        var home = NavMeshWalkableController.Instance.GetRandomHomeTarget();
        var kid = Instantiate(_kidPrefabs[UnityEngine.Random.Range(0, _kidPrefabs.Count)], home, Quaternion.identity);
        kid.Setup(home, UnityEngine.Random.Range(_kidMinHoursOnStreet, _kidMaxHoursOnStreet + 1));
    }
}
