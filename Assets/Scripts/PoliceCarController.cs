using UnityEngine;
using UnityEngine.AI;

public class PoliceCarController : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private Vector3 _target;
    private NavMeshPath _navMeshPath;

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

    private void Awake()
    {
        _navMeshPath = new();
    }
}
