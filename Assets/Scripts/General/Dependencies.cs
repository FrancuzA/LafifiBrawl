using System;
using System.Collections.Generic;
using UnityEngine;

namespace General
{
    [DefaultExecutionOrder(-50)]
    public class Dependencies : MonoBehaviour
    {
        public static Dependencies Instance;
        private Dictionary<Type, object> dependencies = new();

        private void Awake()
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        public void RegisterDependency<T>(T instance)
        {
            if (dependencies.ContainsKey(typeof(T)))
            {
                return;
            }
            dependencies.Add(typeof(T), instance);
        }

        public T GetDependency<T>()
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
}