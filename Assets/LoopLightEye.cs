using DG.Tweening;
using UnityEngine;

public class LoopLightEye : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;
    public float duration = 1f;

    [SerializeField] GameObject player;

    private void Start()
    {
        if (targetLight != null)
        {
            // Forzamos el valor inicial al mínimo
            targetLight.intensity = minIntensity;
            PulseLight();
        }
    }

    private void PulseLight()
    {
        targetLight.DOIntensity(maxIntensity, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void Update()
    {
        transform.LookAt(player.transform);
    }
}
