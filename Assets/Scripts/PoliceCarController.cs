using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.AI;

public class PoliceCarController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private PoliceBustController _bustController;

    [SerializeField] private float _recalculatePathToPlayerEverySeconds;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _chaseAcceleration;
    [SerializeField] private int _chaseAreaInBit;
    [SerializeField] private Rigidbody _rigidbody;

    private Vector3 _target;
    private NavMeshPath _navMeshPath;

    private bool _isAfterVan = false;
    private bool _isVanBusted = false;
    
    private VanController _van;
    public void Setup(Vector3 target)
    {
        if (_navMeshAgent.CalculatePath(target, _navMeshPath))
        {
            Debug.Log($"{GetType()} - Succesfully calculated path to {target}");
            _navMeshAgent.SetPath(_navMeshPath);
        }
        else
        {
            Debug.LogError($"{GetType()} - Could not calculate path to {target}");
        }

        _target = target;
    }

    [Button]
    public void SpottedSteal()
    {
        if (_isAfterVan)
        {
            return;
        }

        _isAfterVan = true;
        _navMeshAgent.speed = _chaseSpeed;
        _navMeshAgent.acceleration = _chaseAcceleration;
        _navMeshAgent.autoTraverseOffMeshLink = true;
        _navMeshAgent.areaMask = 1 << _chaseAreaInBit;

        _bustController.Activate();

        InvokeRepeating(nameof(ChaseVan), 0f, _recalculatePathToPlayerEverySeconds);
    }

    private void ChaseVan()
    {
        if (_isVanBusted)
        {
            return;
        } 

        _navMeshAgent.CalculatePath(_van.transform.position, _navMeshPath);
        _navMeshAgent.SetPath(_navMeshPath);
    }

    private void Awake()
    {
        _navMeshPath = new();
    }

    private void Start()
    {
        _van = FindObjectOfType<VanController>();
        _bustController.VanBusted += VanBustedEventHandler;
    }

    private void VanBustedEventHandler()
    {
        _isVanBusted = true;
        _navMeshAgent.enabled = false;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = _navMeshAgent.velocity;
        _van.Busted();
    }
}
