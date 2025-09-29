using Cysharp.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Video;

public class Sleep : ActivityBase
{
    [SerializeField] private PlayableDirector _timeLineController;

    [SerializeField] private FadeScreen _fadeScreen;

    [SerializeField] private SceneChanger _sceneManager;

    [SerializeField] private VideoPlayer _video;

    [SerializeField] private bool isChangeScene = true;

    private void Start()
    {
        if(_video != null)
        {
            _video.loopPointReached += ChangeSceneAfterVideo;
        }      
    }
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

        if (_video != null)
        {
            _fadeScreen.FadeOut();
            _video.Play();
        }                     
        
        if (isChangeScene)
        {
            await UniTask.Delay(3000);
            _sceneManager.ChangeScene();
        }                
    }

    public void ChangeSceneAfterVideo(VideoPlayer vp)
    {
        _sceneManager.ChangeScene();
    }
}
