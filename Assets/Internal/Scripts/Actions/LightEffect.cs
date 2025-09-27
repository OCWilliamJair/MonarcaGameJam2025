using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightEffectsUniTask : MonoBehaviour
{
    private Light targetLight;
    private Tween currentTween;
    private CancellationTokenSource cts;

    [Header("Configuración base")]
    public float defaultIntensity = 1f;
    public Color defaultColor = Color.white;

    private void Awake()
    {
        targetLight = GetComponent<Light>();
        targetLight.intensity = defaultIntensity;
        targetLight.color = defaultColor;
    }

    private void StopAllEffects()
    {
        cts?.Cancel();
        cts = new CancellationTokenSource();

        // Cancelar tween activo
        if (currentTween != null && currentTween.IsActive())
            currentTween.Kill();
    }

    public void ChangeToRed(float duration = 0.5f)
    {
        StopAllEffects();
        currentTween = targetLight.DOColor(Color.red, duration);
    }

    public void ResetLight(float duration = 0.5f)
    {
        StopAllEffects();
        Sequence s = DOTween.Sequence();
        s.Append(targetLight.DOColor(defaultColor, duration));
        s.Join(targetLight.DOIntensity(defaultIntensity, duration));
        currentTween = s;
    }

    public void StartFlicker(float minInterval = 0.05f, float maxInterval = 0.3f)
    {
        StopAllEffects();
        FlickerLoop(minInterval, maxInterval, cts.Token).Forget();
    }

    private async UniTaskVoid FlickerLoop(float minInterval, float maxInterval, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            targetLight.enabled = !targetLight.enabled;
            float delay = Random.Range(minInterval, maxInterval);
            await UniTask.Delay(System.TimeSpan.FromSeconds(delay), cancellationToken: token);
        }
    }

    public void StartColorCycle(float duration = 1f)
    {
        StopAllEffects();
        ColorCycleLoop(duration, cts.Token).Forget();
    }

    private async UniTaskVoid ColorCycleLoop(float duration, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Color randomColor = Random.ColorHSV();
            targetLight.DOColor(randomColor, duration);
            await UniTask.Delay(System.TimeSpan.FromSeconds(duration), cancellationToken: token);
        }
    }

    public void StartBreathing(float minIntensity = 0.5f, float maxIntensity = 2f, float duration = 2f)
    {
        StopAllEffects();
        currentTween = targetLight.DOIntensity(maxIntensity, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }


    public void FadeOut(float duration = 2f)
    {
        StopAllEffects();
        currentTween = targetLight.DOIntensity(0f, duration)
            .OnComplete(() => targetLight.enabled = false);
    }
}
