using Cysharp.Threading.Tasks;
using UnityEngine;

public class WashCup : ActivityBase
{
    [SerializeField] private GameObject modelCup;

    [SerializeField] private SoundData soundWash;
    [SerializeField] private SoundData soundTakeCup;

    public override void ActivityProcess()
    {
        WashCupRoutine().Forget();
    }

    async UniTask WashCupRoutine()
    {
        modelCup.SetActive(true);
        PlayerActionBlocker.Instance.BlockAction(PlayerActionBlocker.PlayerAction.Move);
        AudioManager.Instance.Play(soundWash.name, modelCup.transform.position);
        await UniTask.Delay(5000);
        AudioManager.Instance.Play(soundTakeCup.name);
        modelCup.SetActive(false);
        PlayerActionBlocker.Instance.UnblockAction(PlayerActionBlocker.PlayerAction.Move);
        CompleteActivity();
    }
}
