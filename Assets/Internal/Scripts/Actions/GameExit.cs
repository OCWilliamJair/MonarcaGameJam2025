using DG.Tweening;
using UnityEngine;

public class GameExit : MonoBehaviour
{
    [Header("Configuración de Fade")]
    public float fadeDuration = 1f;  // Tiempo de fade in/out
    public float minAlpha = 0.2f;    // Transparencia mínima
    public float maxAlpha = 1f;      // Transparencia máxima

    private CanvasGroup canvasGroup;

    /// <summary>
    /// Llama este método para cerrar el juego
    /// </summary>
    /// 

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        FadeLoop();
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // detiene el juego en el Editor
#else
            Application.Quit(); // cierra el juego en build
#endif
    }

    void FadeLoop()
    {
        canvasGroup.alpha = maxAlpha;
        canvasGroup.DOFade(minAlpha, fadeDuration)
            .SetLoops(-1, LoopType.Yoyo); // Yoyo = va y vuelve infinitamente
    }
}
