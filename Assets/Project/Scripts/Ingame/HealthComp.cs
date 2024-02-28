using System;
using Sirenix.OdinInspector;
using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public class HealthComp : MonoBehaviour
    {
        [SerializeField] private float _maxHealth;
        
        public float CurrentHealth { get; private set; }

        private void Awake()
        {
            CurrentHealth = _maxHealth;
        }
        
        [Button]
        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            MLog.Debug($"{CurrentHealth}");
        }

        public bool IsDead()
        {
            return CurrentHealth <= 0f;
        }
    }
}