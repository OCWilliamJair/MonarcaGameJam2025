using UnityEngine;

public class AlarmON : ActivityBase
{
    [SerializeField] private SoundData _ONSound;

    public override void ActivityProcess()
    {
        StopAlarm();
    }
    public void StartAlarm()
    {
        AudioManager.Instance.Play(_ONSound.name, transform.position);
    }

    public void StopAlarm()
    {
        AudioManager.Instance.Stop(_ONSound.name);
        CompleteActivity();
    }
}
