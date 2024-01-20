using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.AI;

public class KidController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject _holdingGameObject;
    [SerializeField] private GameObject _mark;

    private KidState _kidState;
    private NavMeshTargetType _navMeshTargetType;

    private NavMeshPath _navMeshPath;
    
    public GameObject HoldingGameObject => _holdingGameObject;
    
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
        SetPath(NavMeshTargetType.Home);

        UnmarkForSteal();

        var stealedObject = _holdingGameObject;
        _holdingGameObject = null;
        return stealedObject;
    }

    private void Awake()
    {
        _navMeshPath = new();
        _navMeshTargetType = NavMeshTargetType.Path;
        _kidState = KidState.Walking;
        UnmarkForSteal();
        InvokeRepeating(nameof(HandleNavMeshAgent), 1f, 1f);
    }


    private void HandleNavMeshAgent()
    {
        if (!_navMeshAgent.pathPending 
            && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance 
            && _navMeshTargetType == NavMeshTargetType.Path)
        {
            if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                SetPath(NavMeshTargetType.Path);
            }
        }
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
            target = NavMeshWalkableController.Instance.GetRandomHomeTarget();
        }

        if (_navMeshAgent.CalculatePath(target, _navMeshPath))
        {
            Debug.LogError($"{GetType()} - Succesfully calculated path to {target}");
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
