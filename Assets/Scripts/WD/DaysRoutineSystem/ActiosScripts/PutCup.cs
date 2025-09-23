using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class PutCup : ActivityBase
{
    [SerializeField] private SoundData sound;

    [SerializeField] private GameObject modelCup;

    [SerializeField] private GameObject coffeModel;

    public override void ActivityProcess()
    {
        ServingCoffee().Forget();
    }

    protected override void RestartValues()
    {
        base.RestartValues();
        modelCup.SetActive(false);
        coffeModel.SetActive(false);
    }

    async UniTask ServingCoffee()
    {
        modelCup.SetActive(true);
        AudioManager.Instance.Play(sound.name);
        await UniTask.Delay(3000);       
        coffeModel.SetActive(true);
        CompleteActivity();
    }
}
