using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    [Header("Fade Settings")]
    public Image fadePanel;   // Asigna el panel (Image con color negro o blanco)
    public float duration = 1f;

    private void Awake()
    {
        if (fadePanel != null)
        {
            // Asegurar que el panel esté visible al inicio (si lo necesitas)
            fadePanel.raycastTarget = false;
            FadeIn();
        }
    }

    /// <summary>
    /// Hace un fade a negro/blanco (depende del color del panel).
    /// </summary>
    public void FadeIn()
    {
        if (fadePanel != null)
            fadePanel.DOFade(1f, duration);
        Debug.Log("Cerrando");
    }

    /// <summary>
    /// Hace un fade a transparente.
    /// </summary>
    public void FadeOut()
    {
        if (fadePanel != null)
            fadePanel.DOFade(0f, duration);
        Debug.Log("Abriendo");
    }
}
