using System;
using UnityEngine;
using UnityEngine.Events;

public class DayManager : Singletons<DayManager>
{
    public int currentDay = 1;

    public static Action OnCompleteDay;


    public void CompleteDay()
    {
        currentDay++;
        OnCompleteDay?.Invoke();
    }
}
