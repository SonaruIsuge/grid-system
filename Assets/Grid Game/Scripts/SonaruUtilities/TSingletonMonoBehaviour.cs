using UnityEngine;
using System.Collections.Generic;


public class TSingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    private static bool isApplicationQuitting = false;

    // Returns the currently assigned singleton instance without creating a new one.
    // May be null if no instance exists or the application is quitting.
    protected static T ExistingInstance => instance;

    // Avoid creating or returning new singleton instances during shutdown.
    protected static bool IsApplicationQuiting => isApplicationQuitting;
    
    public static T Instance => GetInstance();

    private static T GetInstance()
    {
        if (isApplicationQuitting)
            return null;

        if (instance == null)
        {
            instance = FindAnyObjectByType<T>();
            if (instance == null)
            {
                var obj = new GameObject(typeof(T).Name);
                instance = obj.AddComponent<T>();
                DontDestroyOnLoad(obj);
            }
        }
        return instance;
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
            isApplicationQuitting = true;
        }

    }

    protected void OnApplicationQuit()
    {
        isApplicationQuitting = true;
        instance = null;
    }
}
