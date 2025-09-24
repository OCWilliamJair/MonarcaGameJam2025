using Cysharp.Threading.Tasks;
using UnityEngine;

public class OffPC : ActivityBase
{
    [SerializeField] private SoundData buttonSound;

    [SerializeField] private SoundData pcOff;

    [SerializeField] private GameObject ledPCOff;

    [SerializeField] private GameObject DesktopCanvasPC;
    public override void ActivityProcess()
    {
        base.ActivityProcess();
        StartedPCRoutine().Forget();
    }

    protected override void RestartValues()
    {
        base.RestartValues();
        DesktopCanvasPC.SetActive(false);
        ledPCOff.SetActive(false);
    }

    async UniTask StartedPCRoutine()
    {
        AudioManager.Instance.Play(buttonSound.name, transform.position);
        ledPCOff.SetActive(true);
        await UniTask.Delay(2000);
        AudioManager.Instance.Play(ledPCOff.name, transform.position);
        DesktopCanvasPC.SetActive(false);
        CompleteActivity();
    }
}
