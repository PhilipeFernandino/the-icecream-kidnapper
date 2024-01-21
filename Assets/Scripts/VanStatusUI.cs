using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VanStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _statusText;
    [SerializeField] private GameObject _statusContainer;
    [SerializeField] private UIFinalScreen _screen;

    private VanController _van;
    private CancellationTokenSource _statusTextToken;
    private bool _busted = false;

    private void Start()
    {
        _van = FindObjectOfType<VanController>();
        _van.Busted += VanBustedEventHandler;
        _van.Spotted += VanSpottedEventHandler;
    }

    private async void VanSpottedEventHandler()
    {
        if (_busted)
        {
            return;
        }

        _statusText.text = "You have been spotted stealing";
        _statusContainer.gameObject.SetActive(true);
        
        _statusTextToken?.Cancel();
        _statusTextToken = new();
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2f), cancellationToken: _statusTextToken.Token);
            _statusContainer.gameObject.SetActive(false);
        }
        catch(Exception ex)
        {

        }
    }

    private async void VanBustedEventHandler()
    {
        if (_busted)
        { 
            return; 
        }

        _busted = true;

        _statusText.text = "Busted";
        _statusContainer.gameObject.SetActive(true);

        _statusTextToken?.Cancel();
        _statusTextToken = new();

        await UniTask.Delay(TimeSpan.FromSeconds(4f));

        _statusContainer.gameObject.SetActive(false);
        _screen.Setup();
    }
}
