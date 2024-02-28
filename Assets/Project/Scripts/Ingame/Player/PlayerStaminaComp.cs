using System;
using StartledSeal.Common;
using SuperMaxim.Messaging;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerStaminaComp : MonoBehaviour
    {
        [SerializeField] private float _maxStamina = 100f;
        [SerializeField] private float _staminaRestoreTimeOffset = 1f;
        [SerializeField] private float _staminaRestoreAmountPerSec = 10f;

        public event Action RunOutStamina;
        
        private float _currentStamina;
        private float _lastStaminaConsumeTime;
        
        public float CurrentStamina
        {
            get => _currentStamina;

            private set
            {
                _currentStamina = value;
                PublishStaminaMessage();
            }
        }

        private void Awake()
        {
            CurrentStamina = _maxStamina;
        }

        private void Update()
        {
            CheckToRestoreStamina();
        }

        private void CheckToRestoreStamina()
        {
            if (CurrentStamina < _maxStamina
                && Time.time > _lastStaminaConsumeTime + _staminaRestoreTimeOffset)
            {
                CurrentStamina = Mathf.Clamp(CurrentStamina + _staminaRestoreAmountPerSec * Time.deltaTime, 0f, _maxStamina);
            }
        }

        public void ConsumeStamina(float value)
        {
            if (CurrentStamina < 0f) return;

            _lastStaminaConsumeTime = Time.time;
            CurrentStamina = Mathf.Clamp(CurrentStamina - value, 0f, _maxStamina);
            if (CurrentStamina == 0f)
                RunOutStamina?.Invoke();
        }
        
        private void PublishStaminaMessage()
        {
            MLog.Debug("PlayerStaminaComp", $"{CurrentStamina} - {_maxStamina}");
            
            var payload = new PlayerStaminaPayload()
            {
                CurrentStamina = CurrentStamina,
                MaxStamina = _maxStamina
            };

            Messenger.Default.Publish(payload);
        }
    }
}