using UnityEngine;
using UnityEngine.AI;

public class KidController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    [SerializeField] private GameObject _holdingGameObject; 

    public GameObject HoldingGameObject => _holdingGameObject;

    private KidState _kidState;

    public void MarkForSteal()
    {

    }

    public void UnmarkForSteal()
    {

    }

    public void Steal()
    {

    }

    public enum KidState
    {
        Crying,
        Walking
    }
}
