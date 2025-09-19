using UnityEngine;

public class Singletons<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Busca en la escena
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    // Crea un GameObject si no existe
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    // Unity moderno prefiere OnApplicationQuit
    protected virtual void OnApplicationQuit()
    {
        _instance = null;
    }
}
