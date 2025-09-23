using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class Awakening : ActivityBase
{
    void Start()
    {
        StartActivity();
    }

    public async UniTask AwekeningRoutine()
    {
        await UniTask.Delay(5000);
        CompleteActivity();
    }

    public void StartRoutine()
    {
        AwekeningRoutine().Forget();
    }
}
