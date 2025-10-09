using System;
using System.Collections.Generic;
using UnityEngine;


public class Dependencies : MonoBehaviour
{
    public static Dependencies Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private Dictionary<Type, object> dependencies = new();

    public void RegisterDependency<T>(T instance)
    {
        if (dependencies.ContainsKey(typeof(T)))
        {
            return;
        }
        dependencies.Add(typeof(T), instance);
    }

    public T GetDependancy<T>()
    {
        if (dependencies.TryGetValue(typeof(T), out var value))
        {
            return (T)value;
        }

        return default;
    }

    public void UnregisterDependency<T>()
    {
        if (dependencies.ContainsKey(typeof(T)))
        {
            dependencies.Remove(typeof(T));
        }
    }
}