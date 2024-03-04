using System;
using KBCore.Refs;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerFlashLightController : ValidatedMonoBehaviour
    {
        [SerializeField, Self] PlayerController _playerController;
        
        [SerializeField, Child] private Light _flashLight;
        [SerializeField, Anywhere] private InputReader _input;
        [SerializeField] private float _maxBattery = 100f;
        [SerializeField] private float _batteryConsumePerSec = 10f;

        private float _currentBattery;
        private Animator _animator;
        private int _rightArmAnimatorLayerIndex;
        
        public float CurrentBattery
        {
            get => _currentBattery;

            private set
            {
                _currentBattery = value;
                PublishBatteryMessage();
            }
        }

        private void Awake()
        {
            _animator = _playerController.Animator;
            _rightArmAnimatorLayerIndex = _animator.GetLayerIndex(Const.RightArmAnimatorLayerIndex);
            
            _input.Attack += ToggleLight;
            _currentBattery = _maxBattery;
        }

        private void OnDestroy()
        {
            _input.Attack -= ToggleLight;
        }

        private void Update()
        {
            CheckToConsumeBattery();
        }

        private void CheckToConsumeBattery()
        {
            if (CurrentBattery < 0f) return;
            if (!_flashLight.gameObject.activeSelf) return;
            
            CurrentBattery = Mathf.Clamp(
                CurrentBattery - _batteryConsumePerSec * Time.deltaTime, 
                0f, _maxBattery);
            
            if (CurrentBattery <= 0f)
                TurnOffLight();
        }
        
        private void PublishBatteryMessage()
        {
            var payload = new PlayerBatteryPayload()
            {
                CurrentBattery = CurrentBattery,
                MaxBattery = _maxBattery
            };

            Messenger.Default.Publish(payload);
        }

        private void ToggleLight()
        {
            _flashLight.gameObject.SetActive(!_flashLight.gameObject.activeSelf);
            _animator.SetLayerWeight(_rightArmAnimatorLayerIndex, _flashLight.gameObject.activeSelf ? 1.0f : 0f);
        }

        [Button]
        private void TurnOnLight() => _flashLight.gameObject.SetActive(true);
        
        [Button]
        private void TurnOffLight() => _flashLight.gameObject.SetActive(false);
    }
}