using UnityEngine;

public class LogSystem : Singleton<LogSystem>
{
    [SerializeField] private bool _log;

    private void Awake()
    {
        Debug.unityLogger.logEnabled = _log;
    }
}
