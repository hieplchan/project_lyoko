using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace StartledSeal
{
    public abstract class EventListener<T> : MonoBehaviour
    {
        [SerializeField] private EventChannel<T> _eventChannel;
        [SerializeField] private UnityEvent<T> _unityEvent;

        private void Awake() => _eventChannel.Register(this);

        private void OnDestroy() => _eventChannel.Deregister(this);

        public void Raise(T value)
        {
            _unityEvent.Invoke(value);
        }
    }
    
    public class EventListener : EventListener<Empty> { }
}