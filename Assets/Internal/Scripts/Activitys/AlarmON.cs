using UnityEngine;

public class AlarmON : ActivityBase
{
    [SerializeField] private SoundData _ONSound;

    [SerializeField] private GameObject _light;

    [SerializeField] private LoopLightEye _offLight;

    private void Start()
    {
        StartAlarm();
    }
    public override void ActivityProcess()
    {
        StopAlarm();
    }
    public void StartAlarm()
    {
        AudioManager.Instance.Play(_ONSound.name, transform.position);
        _light.SetActive(true);
        _offLight.PulseLight();
    }

    public void StopAlarm()
    {
        AudioManager.Instance.Stop(_ONSound.name);
        _light.SetActive(false);
        CompleteActivity(); 
    }
}
