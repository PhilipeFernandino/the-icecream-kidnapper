using System.Collections.Generic;
using UnityEngine;

public class DaysSystem : Singleton<DaysSystem>
{
    [SerializeField] private List<DayProperties> _dayProperties;

    private static int _currentDayIndex = 0;


}
