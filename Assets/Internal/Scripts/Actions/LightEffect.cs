using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightEffectsUniTask : MonoBehaviour
{
    private Light _light;
    private bool canTurnOn = true;
    private Tween currentTween;

    private void Awake()
    {
        _light = GetComponent<Light>();
    }

    public void TurnOff()
    {
        currentTween?.Kill();
        _light.enabled = false;
    }

    public void TurnOn()
    {
        if (!canTurnOn) return;
        _light.enabled = true;
    }

    public void SetLock(bool value)
    {
        canTurnOn = !value;
    }

    public void SetColor(Color color)
    {
        _light.color = color;
    }

    public void flick()
    {
        Flicker();
    }

    public void flickIrregular()
    {
        IrregularFlick();
    }
    public async void Flicker(float minIntensity = 0f, float maxIntensity = 3f, float duration = 0.1f, float totalTime = 6f)
    {
        if (!_light.enabled) _light.enabled = true;

        float elapsed = 0f;

        while (elapsed < totalTime)
        {
            _light.intensity = Random.Range(minIntensity, maxIntensity);
            await UniTask.Delay((int)(duration * 1000));
            elapsed += duration;
        }

        _light.intensity = maxIntensity;
    }

    public void Pulse(float pulseIntensity = 4f, float pulseDuration = 0.2f, int loops = 10)
    {
        currentTween?.Kill();
        _light.enabled = true;
        currentTween = _light.DOIntensity(_light.intensity * pulseIntensity, pulseDuration)
                              .SetLoops(loops * 2, LoopType.Yoyo);
    }

    public void IrregularFlick(float minIntensity = 0f, float maxIntensity = 2f, float durationMin = 0.05f, float durationMax = 0.3f, int loops = 30)
    {
        currentTween?.Kill();
        _light.enabled = true;

        Sequence seq = DOTween.Sequence();
        for (int i = 0; i < loops; i++)
        {
            float targetIntensity = Random.Range(minIntensity, maxIntensity);
            float dur = Random.Range(durationMin, durationMax);
            seq.Append(_light.DOIntensity(targetIntensity, dur));
        }
        seq.OnComplete(() => _light.intensity = maxIntensity);
        currentTween = seq;
    }

    public void TurnRed(float duration = 0.5f)
    {
        currentTween?.Kill();
        _light.enabled = true;
        currentTween = _light.DOColor(Color.red, duration);
    }
}
