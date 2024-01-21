using System;
using UnityEngine;

public class TriggerInterface : MonoBehaviour
{
    public event Action<Collider> Triggered;
    public event Action<Collider> Untriggered;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);    
    }

    private void OnTriggerEnter(Collider other)
    {
        Triggered?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        Untriggered?.Invoke(other);
    }
}
