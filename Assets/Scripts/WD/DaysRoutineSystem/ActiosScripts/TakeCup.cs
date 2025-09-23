using UnityEngine;
using UnityEngine.Events;

public class TakeCup : ActivityBase
{
    public override void Interact()
    {
        if(!canInteracte) return;

        base.Interact();

        gameObject.SetActive(false);    
        
        CompleteActivity();
    }

    protected override void RestartValues()
    {
        base.RestartValues();
        gameObject.SetActive(true);
    }

}
