using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class OnPC : ActivityBase
{
    [SerializeField] private SoundData buttonSound;

    [SerializeField] private SoundData pcStarted;

    [SerializeField] private GameObject ledPC;

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
        ledPC.SetActive(false);
    }

    async UniTask StartedPCRoutine()
    {
        AudioManager.Instance.Play(buttonSound.name, transform.position);
        ledPC.SetActive(true);
        await UniTask.Delay(8000);
        AudioManager.Instance.Play(pcStarted.name, transform.position);
        DesktopCanvasPC.SetActive(true);
        CompleteActivity();
    }
}
