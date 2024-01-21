using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class PoliceDispatcher : Singleton<PoliceDispatcher>
{
    [SerializeField] private PoliceCarController _policeCarPrefab;
    [SerializeField] private AnimationCurve _policeSpawnPerCriminalityRatePerHour;
    [SerializeField] private int _criminalityRate;
    
    private int _spawned = 0;

    private void Awake()
    {
        DaytimeSystem.Instance.HourPassed += HourPassedEventHandler;
        Dispatch();
    }

    private void HourPassedEventHandler()
    {
        Dispatch();
    }

    private void Dispatch()
    {
        int spawn = Mathf.CeilToInt(_policeSpawnPerCriminalityRatePerHour.Evaluate(_criminalityRate));
        for (int i = 0; i < spawn; i++)
        {
            DispatchTask();
        }
    }

    private async void DispatchTask()
    {
        await UniTask.Delay(
            TimeSpan.FromSeconds(
                UnityEngine.Random.Range(
                    0f, 
                    DaytimeSystem.Instance.HourDurationInRealSeconds)));

        SpawnRandom();
    }

    private void SpawnRandom()
    {
        _spawned++;
        var drivePath = NavMeshDriveableController.Instance.GetRandomPath();
        var policeCar = Instantiate(_policeCarPrefab, drivePath.Start, Quaternion.identity);
        policeCar.Setup(drivePath.Target);
    }

    public void IncreaseCriminalityRate()
    {
        _criminalityRate++;
    }
}
