using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-2000)]
public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>, new()
{
    private static T _instance;
    private static bool _isApplicationQuitting = false;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if (_isApplicationQuitting)
            {
                Debug.LogWarning($"Trying to access the singleton of type: {typeof(T).Name} when the application is shutting down, this is not recommended.");
                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    GameObject newSingletonObject = new GameObject(typeof(T).Name);
                    _instance = newSingletonObject.AddComponent<T>();
                }
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = GetComponent<T>();

        _instance.transform.SetParent(null);
        DontDestroyOnLoadManager.DontDestroyOnLoad(_instance.gameObject);
    }

    protected virtual void Start()
    {

    }

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }
}
