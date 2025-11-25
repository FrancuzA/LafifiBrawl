using System;
using System.Collections.Generic;
using UnityEngine;

namespace General
{
    [DefaultExecutionOrder(-50)]
    public class Dependencies : MonoBehaviour
    {
        public static Dependencies Instance;
        private readonly Dictionary<Type, object> _dependencies = new();

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void RegisterDependency<T>(T instance)
        {
            if (_dependencies.ContainsKey(typeof(T)))
            {
                return;
            }
            _dependencies.Add(typeof(T), instance);
        }

        public T GetDependency<T>()
        {
            if (_dependencies.TryGetValue(typeof(T), out var value))
            {
                return (T)value;
            }
            return default;
        }

        public void UnregisterDependency<T>()
        {
            if (_dependencies.ContainsKey(typeof(T)))
            {
                _dependencies.Remove(typeof(T));
            }
        }
    }
}