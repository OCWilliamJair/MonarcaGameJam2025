using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class PlayVideo : MonoBehaviour
{

    [SerializeField] private SoundData _sound;

    [Header("References")]
    public VideoPlayer videoPlayer;
    public Material quadMaterial;

    [Header("Textures")]
    public Texture baseImage;
    public RenderTexture videoTexture;

    [Header("Video Settings")]
    public int repeatCount = 1;

    private int currentPlays = 0;
    private bool infiniteLoop = false;

    [SerializeField] private UnityEvent OnStart;
    [SerializeField] private UnityEvent OnEnd;

    private void Start()
    {
        // Mostrar la imagen base al inicio
        if (quadMaterial != null && baseImage != null)
            quadMaterial.SetTexture("_BaseMap", baseImage);

        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;

        infiniteLoop = repeatCount == 0;
    }

    public void PlayVideoClip()
    {
        if (quadMaterial != null && videoTexture != null)
        {
            currentPlays = 0;

            // Cambiar a la textura del video y reproducir
            quadMaterial.SetTexture("_BaseMap", videoTexture);
            videoPlayer.Play();
            AudioManager.Instance.Play(_sound.name, transform.position);
            OnStart.Invoke();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        currentPlays++;

        if (infiniteLoop || currentPlays < repeatCount)
        {
            vp.Play();
        }
        else
        {
            if (quadMaterial != null && baseImage != null)
            {
                quadMaterial.SetTexture("_BaseMap", baseImage);
                AudioManager.Instance.Stop(_sound.name);
                OnEnd.Invoke();
            }
        }
    }
}

