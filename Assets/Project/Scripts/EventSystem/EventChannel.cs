using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace StartledSeal
{
    public abstract class EventChannel<T> : ScriptableObject
    {
        private readonly HashSet<EventListener<T>> _observers = new();
        
        public void Invoke(T value)
        {
            foreach (var observer in _observers)
            {
                observer.Raise(value);
            }
        }

        public void Register(EventListener<T> observer) => _observers.Add(observer);
        public void Deregister(EventListener<T> observer) => _observers.Remove(observer);

#if UNITY_EDITOR
        [Button]
        private void TestPublishValue(T value)
        {
            Invoke(value);
        }
#endif
    }
    
    public readonly struct Empty { }
    
    [CreateAssetMenu(menuName = "StartledSeal/Events/EventChannel")]
    public class EventChannel : EventChannel<Empty> { }
}
    
    