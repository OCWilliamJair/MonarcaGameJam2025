using DG.Tweening;
using UnityEngine;

public class LoopLightEye : MonoBehaviour
{
    [Header("Light Settings")]
    public Light targetLight;
    public float minIntensity = 0.5f;
    public float maxIntensity = 2f;
    public float duration = 1f;

    private Tween pulseTween;

    private void Start()
    {
        if (targetLight != null)
        {
            targetLight.intensity = minIntensity;           
        }
    }

    public void PulseLight()
    {
        pulseTween = targetLight.DOIntensity(maxIntensity, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    public void StopPulse(bool resetToMin = true)
    {
        if (pulseTween != null && pulseTween.IsActive())
        {
            pulseTween.Kill();
        }

        if (targetLight != null && resetToMin)
        {
            targetLight.intensity = minIntensity;
        }
    }

}
