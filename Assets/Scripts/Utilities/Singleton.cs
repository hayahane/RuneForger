using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>, new()
{
    private static T _instance;
    private static bool _isDestorying = false;

    public static T Instance
    {
        get
        {
            if (_isDestorying)
            {
                return null;
            }
            if (_instance == null)
            {
                SetupInstance();
            }

            return _instance;
        }
    }

    private static void SetupInstance()
    {
        _instance = FindObjectOfType<T>();

        if (_instance != null) return;

        var gameObj = new GameObject();
        gameObj.name = typeof(T).Name;

        _instance = gameObj.AddComponent<T>();
        _instance.SetPersistent(gameObj);
    }

    protected virtual void SetPersistent(GameObject gameObj)
    {
    }

    private void RemoveDuplicates()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Awake()
    {
        RemoveDuplicates();
    }

    protected void OnDestroy()
    {
        _isDestorying = true;
    }
}

public class SingletonPersistent<T> : Singleton<T> where T : Singleton<T>, new()
{
    protected override void SetPersistent(GameObject gameObj)
    {
        DontDestroyOnLoad(gameObj);
    }
}