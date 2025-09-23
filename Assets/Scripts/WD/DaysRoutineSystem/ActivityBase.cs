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
    [Header("Configuración de días")]
    public bool canInteracte = false;
    [SerializeField] protected DayEvent[] dayEvents;

    protected override void Awake()
    {
        base.Awake();
        DayManager.OnCompleteDay += RestartValues;
    }

    public virtual void StartActivity()
    {
        canInteracte = true;
    }

    public override void Interact()
    {
        if (!canInteracte) return;
        canInteracte = false;              
    }

    public virtual void CompleteActivity()
    {
        int currentDay = DayManager.Instance.currentDay;
        ChooseDayEvent(currentDay);
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
        canInteracte = false;       
    }
}
