using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Playables;

public class Sleep : ActivityBase
{
    [SerializeField] private PlayableDirector _timeLineController;

    [SerializeField] private FadeScreen _fadeScreen;

    [SerializeField] private SceneChanger _sceneManager;

    private VideoFinal _video;

    [SerializeField] private bool isChangeScene = true;
    public override void ActivityProcess()
    {
        SleepRoutine().Forget();
    }

    async UniTask SleepRoutine()
    {
        _timeLineController.Play();
        await UniTask.Delay(9200);
        _fadeScreen.FadeIn();
        CompleteActivity();
        await UniTask.Delay(4000);
        _video.LoadAndPlay("final.mp4");        
        if (isChangeScene)
        {
            _sceneManager.ChangeScene();
        }                
    }
}
