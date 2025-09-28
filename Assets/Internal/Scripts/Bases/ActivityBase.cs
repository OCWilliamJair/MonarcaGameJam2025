using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class DayEvent
{
    public int dayNumber;       
    public UnityEvent onInteract; 
}

[RequireComponent(typeof(DialogueContainer))]
public abstract class ActivityBase : InteractableBase
{
    [SerializeField] public string activityDescription;

    [SerializeField] public bool showDialoguestart;

    [SerializeField] public bool showDialogueComplete;

    [Header("Configuración de días")]
    [SerializeField] protected DayEvent[] dayEvents;
    private DialogueContainer dialogueContainer;


    [Header("events")]

    [SerializeField] public UnityEvent OnStartActivity;
    [SerializeField] public UnityEvent OnCompleteActivity;

    protected override void Awake()
    {
        base.Awake();
        dialogueContainer = GetComponent<DialogueContainer>();
        DayManager.OnCompleteDay += RestartValues;
    }

    public virtual void StartActivity()
    {
        SetInteract(true);
        OnStartActivity.Invoke();
        if (showDialoguestart)
        {
            dialogueContainer.StarDialogue();
        }
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

        if (showDialogueComplete)
        {
            dialogueContainer.CompleteDialogue();
        }
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
