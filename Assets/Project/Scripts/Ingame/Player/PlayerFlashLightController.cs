using System;
using DG.Tweening;
using KBCore.Refs;
using Sirenix.OdinInspector;
using StartledSeal.Common;
using SuperMaxim.Messaging;
using UnityEngine;
using VLB;

namespace StartledSeal
{
    public class PlayerFlashLightController : ValidatedMonoBehaviour
    {
        [SerializeField, Self] PlayerController _playerController;
        
        [SerializeField, Child] private Light _flashLight;
        [SerializeField, Child] private VolumetricLightBeamHD _volumetricLightBeam;

        [SerializeField, Anywhere] private InputReader _input;
        [SerializeField] private float _maxBattery = 100f;
        [SerializeField] private float _batteryConsumePerSec = 10f;

        [SerializeField] private float _flashDurationIn = 0.1f;
        [SerializeField] private float _flashDurationOut = 3f;
        [SerializeField] private float _flashValueFrom = 10f;
        [SerializeField] private float _flashValueTo = 1f;

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
            
            // _input.Attack += ToggleLight;
            _input.Attack += OnAttack;
            
            TurnOnLight();
            
            _currentBattery = _maxBattery;
        }
        
        private void OnDestroy()
        {
            // _input.Attack -= ToggleLight;
            _input.Attack -= OnAttack;
        }
        
        [Button]
        private void OnAttack()
        {
            DOVirtual.Float(_flashValueFrom, _flashValueTo, _flashDurationIn, value =>
            {
                _volumetricLightBeam.sideSoftness = value;
            }).onComplete = () =>
            {
                DOVirtual.Float(_flashValueTo, _flashValueFrom, _flashDurationOut, value =>
                {
                    _volumetricLightBeam.sideSoftness = value;
                });
            };
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

        [Button]
        private void TurnLight(bool isOn)
        {
            _flashLight.gameObject.SetActive(isOn);
            _animator.SetLayerWeight(_rightArmAnimatorLayerIndex, isOn ? 1.0f : 0f);
        }

        [Button]
        private void TurnOnLight() => TurnLight(true);
        
        [Button]
        private void TurnOffLight() => TurnLight(false);
        
        [Button]
        private void ToggleLight() => TurnLight(!_flashLight.gameObject.activeSelf);

        [Button]
        private void ResetLight()
        {
            _volumetricLightBeam.sideSoftness = 10;
        }
    }
}