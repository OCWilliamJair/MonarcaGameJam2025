using Cysharp.Threading.Tasks;
using UnityEngine;

public class HeatFood : ActivityBase
{
    [SerializeField] private GameObject foodToHeat;

    [SerializeField] private SoundData microWaveStartSound;

    [SerializeField] private SoundData microWaveProcessSound;

    [SerializeField] private SoundData microWaveReadySound;


    public override void ActivityProcess()
    {
        MicrowaveRoutine().Forget();
    }

    async UniTask MicrowaveRoutine()
    {
        AudioManager.Instance.Play(microWaveStartSound.name, transform.position);
        await UniTask.Delay(1000);
        foodToHeat.SetActive(false);
        AudioManager.Instance.Play(microWaveProcessSound.name, transform.position);
        await UniTask.Delay(10000);
        AudioManager.Instance.Play(microWaveReadySound.name, transform.position);
        CompleteActivity();
    }
}
