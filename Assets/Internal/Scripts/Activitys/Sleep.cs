using UnityEngine;
using UnityEngine.Playables;

public class Sleep : ActivityBase
{
    [SerializeField] private PlayableDirector _timeLineController;
    public override void ActivityProcess()
    {
        _timeLineController.Play();
    }
}
