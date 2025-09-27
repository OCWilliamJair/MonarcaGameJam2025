using UnityEngine;

public class EatFood : ActivityBase
{
    [SerializeField] private SoundData eatingSound;

    public override void ActivityProcess()
    {
        AudioManager.Instance.Play(eatingSound.name);
        CompleteActivity();
    }
}
