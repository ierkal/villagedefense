using System;
using System.Collections.Generic;
using _Scripts.Main;
using _Scripts.Main.Services;

namespace _Scripts.Event
{
    public class EventBroker: MonoSingleton<EventBroker>, IGameService
    {
        public delegate void EventDelegate<in T>(T e) where T : IEvent;

        private readonly Dictionary<Type, Delegate> _delegates = new ();
        
        private void Awake()
        {
            InitializeSingleton();
        }

        private void OnApplicationQuit()
        {
            CleanUp();
        }

        public void AddEventListener<T>(EventDelegate<T> listener) where T : IEvent
        {
            if (_delegates.TryGetValue(typeof(T), out var d))
            {
                _delegates[typeof(T)] = Delegate.Combine(d, listener);
            }
            else
            {
                _delegates[typeof(T)] = listener;
            }
        }

        public void RemoveEventListener<T>(EventDelegate<T> listener) where T : IEvent
        {
            if (!_delegates.TryGetValue(typeof(T), out var d)) return;
            var currentDel = Delegate.Remove(d, listener);

            if (currentDel == null)
            {
                _delegates.Remove(typeof(T));
            }
            else
            {
                _delegates[typeof(T)] = currentDel;
            }
        }

        public void RaiseEvent<T>(T e) where T : IEvent
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (!_delegates.TryGetValue(e.GetType(), out var d))
            {
                return;
            }
            d.DynamicInvoke(e);
        }

        private void CleanUp()
        {
            _delegates.Clear();
        }
    }
}