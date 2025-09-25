using DG.Tweening;
using UnityEngine;

public class PlayerBlink : MonoBehaviour
{
    public Material blinkMaterial;

    [Header("Duraciones")]
    public float closeTime = 0.15f;
    public float stayClosedTime = 0.05f;
    public float openTime = 0.2f;

    private void Start()
    {
        blinkMaterial.SetFloat("_Blink", 0f);
    }

    public void Blink()
    {
        Sequence seq = DOTween.Sequence();

        // Cierre
        seq.Append(DOTween.To(
            () => blinkMaterial.GetFloat("_Blink"),
            x => blinkMaterial.SetFloat("_Blink", x),
            1f, closeTime));

        // Tiempo cerrado
        seq.AppendInterval(stayClosedTime);

        // Apertura
        seq.Append(DOTween.To(
            () => blinkMaterial.GetFloat("_Blink"),
            x => blinkMaterial.SetFloat("_Blink", x),
            0f, openTime));
    }
}
