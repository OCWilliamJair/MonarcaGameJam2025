using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    public void ChangeScene()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadScene(sceneToLoad);
        }
    }

    public void ChangeSceneAsync()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadSceneAsync(sceneToLoad, 1.5f); 
        }
    }
}
