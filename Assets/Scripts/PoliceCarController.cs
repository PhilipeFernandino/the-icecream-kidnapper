using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.AI;
using static KidController;

public class PoliceCarController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private TriggerInterface _bustTrigger;
    [SerializeField] private TriggerInterface _visionTrigger;

    [SerializeField] private float _recalculatePathToPlayerEverySeconds;
    [SerializeField] private float _chaseSpeed;
    [SerializeField] private float _chaseAcceleration;
    [SerializeField] private int _chaseAreaInBit;
    [SerializeField] private Rigidbody _rigidbody;

    private Vector3 _target;
    private NavMeshPath _navMeshPath;

    private bool _isAfterVan = false;
    private bool _isVanBusted = false;
    private bool _isLookingForSuspect = false;

    private VanController _van;

    public void Setup(Vector3 target)
    {
        SetPath(target);
    }

    public void SpottedSuspect()
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

        _bustTrigger.Activate();

        InvokeRepeating(nameof(ChaseVan), 0f, _recalculatePathToPlayerEverySeconds);
    }

    private void RaiseAlarm()
    {
        foreach (var cop in FindObjectsOfType<PoliceCarController>())
        {
            cop.SpottedSuspect();
        }
    }

    public async void SpotKidCrying(KidController kid)
    {
        _isLookingForSuspect = true;
        _visionTrigger.Activate();

        await UniTask.Delay(TimeSpan.FromSeconds(10f));

        _isLookingForSuspect = false;
        _visionTrigger.Deactivate();
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

        _bustTrigger.Deactivate();
        _visionTrigger.Deactivate();

        _bustTrigger.Triggered += VanBustedEventHandler;
        _visionTrigger.Triggered += SuspectSpottedEventHandler;

        InvokeRepeating(nameof(HandleNavMeshAgent), 1f, 1f);
    }

    private void HandleNavMeshAgent()
    {
        if (!_navMeshAgent.pathPending
            && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                SetPath(null);
            }
        }
    }

    private async void SuspectSpottedEventHandler(Collider _)
    {
        SpottedSuspect();

        _van.HasBeenSpotted();

        await UniTask.Delay(TimeSpan.FromSeconds(5));
        RaiseAlarm();
    }

    private async void SetPath(Vector3? targetNullable)
    {
        Vector3 target;

        if (targetNullable != null)
        {
            target = targetNullable.Value;
        }
        else
        {
            target = NavMeshDriveableController.Instance.GetRandomPath().Target;
        }

        if (_navMeshAgent.CalculatePath(target, _navMeshPath))
        {
            Debug.Log($"{GetType()} - Succesfully calculated path to {target}");
            _navMeshAgent.SetPath(_navMeshPath);
        }
        else
        {
            Debug.LogError($"{GetType()} - Could not calculate path to {target}");
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            SetPath(null);
        }
    }

    private void VanBustedEventHandler(Collider _)
    {
        _isVanBusted = true;
        _navMeshAgent.enabled = false;
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = _navMeshAgent.velocity;
        _van.Bust();
    }
}
