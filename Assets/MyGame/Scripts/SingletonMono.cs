using UnityEngine;

public abstract class SingletonMono<T> : MonoBehaviour
    where T : SingletonMono<T>
{
    private static readonly object lockObject = new object();
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        var name    = $"{typeof(T)}(Singleton)";
                        var go      = new GameObject(name);
                        instance    = go.AddComponent<T>();
                        instance.Init();
                        DontDestroyOnLoad(go);
                    }
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            throw new System.Exception($"Singleton is duplicate: {typeof(T)}.");
        }
        else
        {
            instance = this as T;
            instance.Init();
            DontDestroyOnLoad(gameObject);
        }
    }

    protected abstract void Init();
}
