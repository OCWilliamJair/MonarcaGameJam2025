using UnityEngine;
using UnityEngine.Playables;

public class Sleep : InteractableBase
{
    [SerializeField] private PlayableDirector _timeLineController;
    public override void Interact()
    {
        _timeLineController.Play();
    }
}
