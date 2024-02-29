using System;
using System.Collections.Generic;
using KBCore.Refs;
using Sirenix.OdinInspector;
using StartledSeal.Common;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

namespace StartledSeal.Project.Scripts.UI
{
    public class PlayerStaminaSlider : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private CanvasGroup _canvasGroup;
        
        [SerializeField] private float _middleThreshold = 0.5f;
        [SerializeField] private float _timeOutHideUISec = 0.5f;

        [SerializeField] private bool _isAutoHideEnable;
        
        [SerializeField] private List<Slider> _sliderList;
        [SerializeField] private List<Slider> _middleSliderList;

        [SerializeField] private List<Image> _imgBGList;
        [SerializeField] private List<UIOutline> _outlineBGList;

        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _warningColor;
        [SerializeField] private Color _staminaRunOutColor;
        private float _lastValueChangedTime;

        private void Awake()
        {
            Messenger.Default.Subscribe<PlayerStaminaPayload>(HandlePlayerStaminaChanged);
            Messenger.Default.Subscribe<PlayerDeadEventPayload>(HandlePlayerDeadEvent);

            _lastValueChangedTime = Time.time;
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerStaminaPayload>(HandlePlayerStaminaChanged);
            Messenger.Default.Unsubscribe<PlayerDeadEventPayload>(HandlePlayerDeadEvent);
        }

        private void Update()
        {
            CheckToHideUI();
        }

        private void CheckToHideUI()
        {
            if (_isAutoHideEnable)
                _canvasGroup.alpha = Time.time < _lastValueChangedTime + _timeOutHideUISec ? 1 : 0;
        }

        private void HandlePlayerStaminaChanged(PlayerStaminaPayload payload)
        {
            _canvasGroup.alpha = 1;
            
            _lastValueChangedTime = Time.time;
            
            var value = payload.CurrentStamina / payload.MaxStamina;
            var color = value > _middleThreshold ? _normalColor : 
                Color.Lerp(_warningColor, _staminaRunOutColor, 1 - value * 2);
                
            foreach (var slider in _sliderList)
            {
                slider.value = value;
            }

            foreach (var middleSlider in _middleSliderList)
            {
                middleSlider.gameObject.SetActive(value > _middleThreshold);    
            }

            ChangeColor(color);
        }
        
        private void HandlePlayerDeadEvent(PlayerDeadEventPayload payload)
        {
            MLog.Debug("HandlePlayerDeadEvent");
            Messenger.Default.Unsubscribe<PlayerStaminaPayload>(HandlePlayerStaminaChanged);
            _canvasGroup.alpha = 0;
        }

        private void ChangeColor(Color color)
        {
            foreach (var img in _imgBGList)
            {
                img.color = color;
            }

            foreach (var outline in _outlineBGList)
            {
                outline.color = color;
            }
        }

#if UNITY_EDITOR
        [Button]
        private void ApplyNormalColor()
        {
            ChangeColor(_normalColor);
        }
        #endif
    }
}