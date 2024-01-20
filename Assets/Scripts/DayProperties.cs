using UnityEngine;

public class DayProperties : ScriptableObject
{
    [SerializeField] private int _day;

    [Header("Kid Spawn Properties")]
    [SerializeField] private int _spawnKidsChancePerHour;
    [SerializeField] private int _spawnMinKids;
    [SerializeField] private int _spawnMaxKids;

    [Header("Cop Spawn Properties")]
    [SerializeField] private int _spawnCopsChancePerHour;
    [SerializeField] private int _spawnMinCops;
    [SerializeField] private int _spawnMaxCops;

    public int Day => _day;
    public int SpawnKidsChancePerHour => _spawnKidsChancePerHour;
    public int SpawnMinKids => _spawnMinKids;
    public int SpawnMaxKids => _spawnMaxKids;
    public int SpawnCopsChancePerHour => _spawnCopsChancePerHour;
    public int SpawnMinCops => _spawnMinCops;
    public int SpawnMaxCops => _spawnMaxCops;
}
