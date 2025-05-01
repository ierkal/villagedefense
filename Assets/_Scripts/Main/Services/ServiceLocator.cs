using System;
using System.Collections.Generic;
using _Scripts.OdinAttributes;
using _Scripts.Utility;
using UnityEngine;

namespace _Scripts.Main.Services
{
    [LogTag("ServiceLocator")]
    public class ServiceLocator : MonoSingleton<ServiceLocator>
    {
        private readonly Dictionary<Type, IGameService> _services = new();

        private void Awake()
        {
            InitializeSingleton();
        }

        public void Register(Type type, IGameService service)
        {
            if (_services.ContainsKey(type))
            {
                return;
            }

            _services[type] = service;
            

        }
        public T Get<T>() where T : class, IGameService
        {
            if (_services.TryGetValue(typeof(T), out var service))
            {
                return service as T;
            }
            Log.Error(this, $"Service of type {typeof(T).Name} not found.");
            return null;
        }

        public bool Has<T>() where T : class, IGameService
        {
            return _services.ContainsKey(typeof(T));
        }

        public bool Has(Type type)
        {
            return _services.ContainsKey(type);
        }
        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var instance))
            {
                service = instance as T;
                return true;
            }

            service = null;
            return false;
        }

    }
}