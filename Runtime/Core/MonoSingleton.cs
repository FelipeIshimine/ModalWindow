using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static event Action<T> OnInitialized;
    public bool setInactiveOnStart = false;
    public bool dontDestroyOnLoad = false;

    private static T _instance = null;

    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            throw new Exception($"Mono Singleton Instance Error {this}");
        }
        _instance = this as T;
    }

    protected virtual void Start()
    {
        if (setInactiveOnStart)
            gameObject.SetActive(false);

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);

        OnInitialized?.Invoke(Instance);
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }


    public static void DestroyInstance()
    {
        Destroy(_instance.gameObject);
        _instance = null;
    }

    public static void SetActive(bool value) => Instance.gameObject.SetActive(value);

}