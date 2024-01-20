using UnityEngine;

public class PoliceCarSpawner : MonoBehaviour
{
    [SerializeField] private PoliceCarController _policeCarPrefab;

    private void Awake()
    {
        SpawnRandom();
    }

    private void SpawnRandom()
    {
        var drivePath = NavMeshDriveableController.Instance.GetRandomPath();
        var policeCar = Instantiate(_policeCarPrefab, drivePath.Start, Quaternion.identity);
        policeCar.Setup(drivePath.Target);
    }
}
