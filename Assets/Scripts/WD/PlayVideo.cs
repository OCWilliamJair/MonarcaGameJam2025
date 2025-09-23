using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class PlayVideo : MonoBehaviour
{
    private VideoPlayer _videoPlayer;

    [SerializeField] private VideoClip _video;

    [SerializeField] private int timeToOffVideo;

    private void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.clip = _video;       
    }
    async UniTask PlayTemporalVideo()
    {
        _videoPlayer.Play();
        await UniTask.Delay(timeToOffVideo);
    }

    public void StartVideo()
    {
        _videoPlayer.Play();
    }

    public void StopVideo()
    {
        _videoPlayer.Stop();
    }

    public void SetVideo(VideoClip newVideo)
    {
        _videoPlayer.clip = newVideo;
    }

    public void SetTime(int newTime)
    {
        timeToOffVideo = newTime;
    }
}
