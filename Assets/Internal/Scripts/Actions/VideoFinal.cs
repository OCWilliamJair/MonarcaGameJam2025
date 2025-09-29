using UnityEngine;
using UnityEngine.Video;


[RequireComponent(typeof(VideoPlayer), typeof(AudioSource))]
public class VideoFinal : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    private SceneChanger sceneChanger;
    void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        sceneChanger = GetComponent<SceneChanger>();    

        // Configuración básica
        videoPlayer.playOnAwake = false;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane; // Pantalla completa
        videoPlayer.targetCamera = Camera.main;

        // Configuración de audio
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, GetComponent<AudioSource>());

        
    }

    private void OnEnable()
    {
        videoPlayer.loopPointReached += RestartGame;
    }

    private void OnDisable()
    {
        videoPlayer.loopPointReached -= RestartGame;
    }

    public void LoadAndPlay(string videoFileName)
    {
        string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.url = path;

        videoPlayer.prepareCompleted += OnPrepared;
        videoPlayer.Prepare();
    }

    private void OnPrepared(VideoPlayer vp)
    {
        vp.prepareCompleted -= OnPrepared; // evitar dobles llamadas
        vp.Play();
    }

    public void StopVideo() => videoPlayer.Stop();
    public void PauseVideo() => videoPlayer.Pause();
    public void ResumeVideo() => videoPlayer.Play();

    public void RestartGame(VideoPlayer vp)
    {
        sceneChanger.ChangeScene();
    }
}
