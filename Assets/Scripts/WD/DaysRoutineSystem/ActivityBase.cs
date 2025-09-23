using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DayEvent
{
    public int dayNumber;       
    public UnityEvent onInteract; 
}

public abstract class ActivityBase : InteractableBase
{
    [SerializeField] public string activityDescription;

    [Header("Configuración de días")]
    [SerializeField] protected DayEvent[] dayEvents;


    [Header("events")]

    [SerializeField] public UnityEvent OnStartActivity;
    [SerializeField] public UnityEvent OnCompleteActivity;

    protected override void Awake()
    {
        base.Awake();
        DayManager.OnCompleteDay += RestartValues;
    }

    public virtual void StartActivity()
    {
        SetInteract(true);
        OnStartActivity.Invoke();
    }

    public override void Interact()
    {
        if (!canInteract) return;
        SetInteract(false);
        ActivityProcess();
    }

    public virtual void ActivityProcess(){}
    public virtual void CompleteActivity()
    {
        int currentDay = DayManager.Instance.currentDay;
        ChooseDayEvent(currentDay);
        OnCompleteActivity.Invoke();
    }

    /// <summary>
    /// Ejecuta el evento correspondiente al día
    /// </summary>
    protected virtual void ChooseDayEvent(int day)
    {
        foreach (var e in dayEvents)
        {
            if (e.dayNumber == day)
            {
                e.onInteract?.Invoke();
                return;
            }
        }
        Debug.LogWarning($"[ActivityBase] No hay evento configurado para el día {day} en {gameObject.name}");
    }

    protected virtual void RestartValues()
    {
        SetInteract(false);
    }
}
