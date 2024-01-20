using System;
using UnityEngine;

public class DaytimeSystem : Singleton<DaytimeSystem>
{
    [SerializeField] private float _passMinuteEverySeconds;
    [SerializeField] private int _dayStartInHours = 8;
    [SerializeField] private int _dayEndInHours = 20;
    [SerializeField] private Gradient _astroLightFilterGradient;
    [SerializeField] private AnimationCurve _astroLightIntensityCurve;
    [SerializeField] private Light _astroLight;

    private DateTime _currentDayTime;
    private int _dayStartInMinutes;
    private int _dayEndInMinutes;

    public DateTime CurrentDayTime => _currentDayTime;

    public event Action DayEndReached;
    public event Action HourPassed;

    private void Awake()
    {
        _currentDayTime = new DateTime(1970, 1, 1, _dayStartInHours, 0, 0);
        _dayStartInMinutes = _dayStartInHours * 60;
        _dayEndInMinutes = _dayEndInHours * 60;

        InvokeRepeating(nameof(UpdateTime), _passMinuteEverySeconds, _passMinuteEverySeconds);
    }

    private void UpdateTime()
    {
        // End daytime reached, won't update anymore
        if (_currentDayTime.Hour == _dayEndInHours)
        {
            return;
        }

        int lastRegisteredHour = _currentDayTime.Hour;

        _currentDayTime = _currentDayTime.AddMinutes(1);
        
        if (lastRegisteredHour < _currentDayTime.Hour)
        {
            HourPassed?.Invoke();
        }

        if (_currentDayTime.Hour == _dayEndInHours)
        {
            DayEndReached?.Invoke();
        }

        float currentMinutes = _currentDayTime.Hour * 60 + _currentDayTime.Minute;
        float timeOfDayNormalized = (currentMinutes - _dayStartInMinutes) / (_dayEndInMinutes - _dayStartInMinutes);

        _astroLight.intensity = _astroLightIntensityCurve.Evaluate(timeOfDayNormalized);
        _astroLight.color = _astroLightFilterGradient.Evaluate(timeOfDayNormalized);

        Debug.Log($"{GetType()} - H: {_currentDayTime.Hour}, M: {_currentDayTime.Minute}");
    }
}
