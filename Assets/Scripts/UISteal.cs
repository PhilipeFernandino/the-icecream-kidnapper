using System;
using TMPro;
using UnityEngine;

public class UISteal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _stealsCounterText;

    private StealController _stealController;

    private void Awake()
    {
        _stealController = FindObjectOfType<StealController>();
        _stealController.Stealed += StealedEventHandler;
    }

    private void StealedEventHandler(int stealed)
    {
        _stealsCounterText.text = stealed.ToString();
    }
}
