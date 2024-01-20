using System;
using UnityEngine;

public class PoliceBustController : MonoBehaviour
{
    public event Action VanBusted;

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        VanBusted?.Invoke();
    }
}
