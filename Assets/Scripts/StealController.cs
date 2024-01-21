using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StealController : MonoBehaviour
{
    [SerializeField] private BoxCollider _boxCollider;

    private HashSet<KidController> _stealableKids = new();

    private int _stealed = 0;

    public int TotalStealed => _stealed;

    public event Action<int> Stealed; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out KidController kidController))
        {
            if (kidController.HoldingGameObject)
            {
                kidController.MarkForSteal();
                _stealableKids.Add(kidController);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out KidController kidController))
        {
            if (_stealableKids.Contains(kidController))
            {
                kidController.UnmarkForSteal();
                _stealableKids.Remove(kidController);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_stealableKids.Count > 0)
            {
                var stealedObject = _stealableKids.First().Steal();
                _stealed++;
                Stealed?.Invoke(_stealed);
            }
        }
    }
}
