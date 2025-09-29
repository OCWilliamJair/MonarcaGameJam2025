using UnityEngine;
using UnityEngine.Video;


[RequireComponent(typeof(VideoPlayer), typeof(AudioSource))]
public class VideoFinal : MonoBehaviour
{
    [SerializeField] private string videoFileName = "miVideo.mp4";
    [SerializeField] private Camera mainCamera;

    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        string path = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = path;

        // Mostrar en cámara
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = mainCamera;

        videoPlayer.Play();
    }
}
