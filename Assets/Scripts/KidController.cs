using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AI;

public class KidController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject _holdingGameObject;
    [SerializeField] private GameObject _mark;
    [SerializeField] private float _radiusToLookForRandomTarget;
    [SerializeField] private float _cryOutLoudRadius;
    [SerializeField] private float _cryOutLoudEverySeconds;
    [SerializeField] private int _walkAreaMaskBits;
    [SerializeField] private LayerMask _cryOutLoudMask;

    private KidState _kidState;
    private NavMeshTargetType _navMeshTargetType;
    private Vector3 _homePosition;
    private Collider[] _overlapSphereColliders = new Collider[2];

    private int _hoursOnTheStreet = 0;
    private int _maxHoursOnTheStreet;
    private int _walkAreaMask; 

    private bool _isGoingHome = false;

    private NavMeshPath _navMeshPath;

    public GameObject HoldingGameObject => _holdingGameObject;
    
    public void Setup(Vector3 homePosition, int maxHoursOnTheStreet)
    {
        _maxHoursOnTheStreet = maxHoursOnTheStreet;
        _homePosition = homePosition;
    }

    public void MarkForSteal()
    {
        _mark.SetActive(true);
        _navMeshAgent.isStopped = true;

        if (_kidState == KidState.Walking)
        {
            _kidState = KidState.Stopped;
        }
    }

    public void UnmarkForSteal()
    {
        _mark.SetActive(false);
        _navMeshAgent.isStopped = false;

        if (_kidState == KidState.Stopped)
        {
            _kidState = KidState.Walking;
        }
    }

    public GameObject Steal()
    {
        _holdingGameObject.SetActive(false);
        _holdingGameObject.transform.SetParent(null, false);
        
        _kidState = KidState.Crying;
        InvokeRepeating(nameof(CryOutLoud), 0f, _cryOutLoudEverySeconds);
        GoHome();

        UnmarkForSteal();

        var stealedObject = _holdingGameObject;
        _holdingGameObject = null;
        return stealedObject;
    }

    private void CryOutLoud()
    {
        int hits = Physics.OverlapSphereNonAlloc(transform.position, _cryOutLoudRadius, _overlapSphereColliders, _cryOutLoudMask, QueryTriggerInteraction.Ignore);
        for (int i = 0; i < hits; i++)
        {
            if (_overlapSphereColliders[i].TryGetComponent<PoliceCarController>(out PoliceCarController police))
            {
                police.SpotKidCrying(this);
            }
        }
    }

    private void Awake()
    {
        _walkAreaMask = 1 << _walkAreaMaskBits;

        _navMeshAgent.areaMask = _walkAreaMask;

        _navMeshPath = new();
        _navMeshTargetType = NavMeshTargetType.Path;
        _kidState = KidState.Walking;

        DaytimeSystem.Instance.HourPassed += HourPassedEventHandler;
        DaytimeSystem.Instance.DayEndReached += DayEndReachedEventHandler;

        UnmarkForSteal();
        InvokeRepeating(nameof(HandleNavMeshAgent), 1f, 1f);
    }

    private void DayEndReachedEventHandler()
    {
        GoHome();
    }

    private void HourPassedEventHandler()
    {
        _hoursOnTheStreet += 1;
        if (_hoursOnTheStreet == _maxHoursOnTheStreet)
        {
            GoHome();
        }
    }

    private void GoHome()
    {
        SetPath(NavMeshTargetType.Home);
        _isGoingHome = true;
    }

    private void HandleNavMeshAgent()
    {
        if (!_navMeshAgent.pathPending 
            && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                if (_isGoingHome)
                {
                    EnterHome();
                }
                else
                {
                    SetPath(NavMeshTargetType.Path);
                }
            }
        }
    }

    private void EnterHome()
    {
        DaytimeSystem.Instance.HourPassed -= HourPassedEventHandler;
        DaytimeSystem.Instance.DayEndReached-= DayEndReachedEventHandler;
        Destroy(gameObject);
    }

    private async void SetPath(NavMeshTargetType navMeshTargetType)
    {
        Vector3 target;
        if (navMeshTargetType == NavMeshTargetType.Path)
        {
            target = NavMeshWalkableController.Instance.GetRandomPathTarget();
        }
        else
        {
            target = _homePosition;
        }

        if (_navMeshAgent.CalculatePath(target, _navMeshPath))
        {
            Debug.Log($"{GetType()} - Succesfully calculated path to {target}");
            _navMeshAgent.SetPath(_navMeshPath);
            _navMeshTargetType = navMeshTargetType;
        }
        else
        {
            Debug.LogError($"{GetType()} - Could not calculate path to {target}");
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            SetPath(navMeshTargetType);
        }
    }



    public enum KidState
    {
        Crying,
        Walking,
        Stopped,
    }

    public enum NavMeshTargetType 
    {
        Path, 
        Home,
    }
}
