using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] PlayableDirector _director;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Carga una escena de forma directa (sin transición).
    /// </summary>
    /// 
    private void Start()
    {
        if(_director != null)
        {
            _director.Play();
        }
    }
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Carga una escena con transición (con UniTask).
    /// </summary>
    public async UniTaskVoid LoadSceneAsync(string sceneName, float delay = 1f)
    {
        // (Opcional) aquí puedes poner un Fade Out
        Debug.Log("Cambiando de escena en " + delay + " segundos...");

        await UniTask.Delay(Mathf.RoundToInt(delay * 1000));

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Espera a que esté listo al 90%
        while (asyncLoad.progress < 0.9f)
        {
            await UniTask.Yield();
        }

        // Activamos la escena
        asyncLoad.allowSceneActivation = true;

        // (Opcional) aquí puedes poner un Fade In
    }
}
