using System;
using Sirenix.OdinInspector;
using StartledSeal.Common;
using SuperMaxim.Messaging;
using UnityEngine;

namespace StartledSeal
{
    public class HealthComp : MonoBehaviour
    {
        [SerializeField] private float _maxHealth;
        
        [Header("Message Publish Setting")]
        [SerializeField] private bool _isNeedToPublishMessage;
        [field: SerializeField] public string Tag { get; private set; }
        
        private float _currentHealth;
        
        public void SetTag(string tagName)
        {
            Tag = tagName;
        }
        
        public float CurrentHealth
        {
            get => _currentHealth;

            private set
            {
                _currentHealth = value;
                if (_isNeedToPublishMessage)
                    PublishHealthMessage();
            }
        }

        private void Awake()
        {
            CurrentHealth = _maxHealth;
        }
        
        [Button]    
        public void TakeDamage(float damage)
        {
            CurrentHealth -= damage;
            MLog.Debug($"CurrentHealth {CurrentHealth}");
        }
        
        private void PublishHealthMessage()
        {
            var payload = new HealthPayload()
            {
                Tag = Tag,
                CurrentHealth = CurrentHealth,
                MaxHealth = _maxHealth,
            };

            Messenger.Default.Publish(payload);
        }

        public bool IsDead()
        {
            return CurrentHealth <= 0f;
        }
    }
}