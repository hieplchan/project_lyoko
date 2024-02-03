using System;
using StartledSeal.Common;
using Unity.VisualScripting;
using UnityEngine;

namespace StartledSeal.Utils.Extension
{
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    MLog.Error("MonoSingleton", typeof(T) + " is null");
                
                return _instance;                    
            }
        }

        protected virtual void Awake()
        {
            _instance = this as T;
        }
    }
}