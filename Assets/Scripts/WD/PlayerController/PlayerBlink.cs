using DG.Tweening;
using UnityEngine;

public class PlayerBlink : MonoBehaviour
{
    [Header("Eyelid Settings")]
    public RectTransform upperEyelid;
    public RectTransform lowerEyelid;
    public float blinkDuration = 0.1f;
    public int blinkCount = 1;
    public float closedOffset = 0f; // cuánto se desplazan los párpados para cerrar completamente

    private Vector2 upperStartPos;
    private Vector2 lowerStartPos;

    private void Awake()
    {
        upperStartPos = upperEyelid.anchoredPosition;
        lowerStartPos = lowerEyelid.anchoredPosition;
    }

    void Start()
    {
        InvokeRepeating(nameof(Blink), Random.Range(3f, 6f), Random.Range(5f, 10f));
    }

    public void Blink()
    {
        Sequence blinkSequence = DOTween.Sequence();

        for (int i = 0; i < blinkCount; i++)
        {
            // Subir y bajar párpados para cerrar el ojo
            blinkSequence.Append(
                DOTween.Sequence()
                .Join(upperEyelid.DOAnchorPosY(closedOffset, blinkDuration))
                .Join(lowerEyelid.DOAnchorPosY(-closedOffset, blinkDuration))
            );

            // Volver a abrir
            blinkSequence.Append(
                DOTween.Sequence()
                .Join(upperEyelid.DOAnchorPosY(upperStartPos.y, blinkDuration))
                .Join(lowerEyelid.DOAnchorPosY(lowerStartPos.y, blinkDuration))
            );
        }
    }
}
