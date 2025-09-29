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
                _instance = FindFirstObjectByType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                    //DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    protected virtual void OnApplicationQuit()
    {
        _instance = null;
    }
}
