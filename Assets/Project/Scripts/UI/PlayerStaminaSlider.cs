using System;
using System.Collections.Generic;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

namespace StartledSeal.Project.Scripts.UI
{
    public class PlayerStaminaSlider : MonoBehaviour
    {
        [SerializeField] private float _middleThreshold = 0.5f;
        [SerializeField] private float _timeOutHideUISec = 0.5f;
        
        [SerializeField] private List<Slider> _sliderList;
        [SerializeField] private List<Slider> _middleSliderList;

        [SerializeField] private List<Image> _imgImageList;

        [SerializeField] private Color _normalColor;
        [SerializeField] private Color _warningColor;
        [SerializeField] private Color _staminaRunOutColor;
        private float _lastValueChangedTime;

        private void Awake()
        {
            Messenger.Default.Subscribe<PlayerStaminaPayload>(HandlePlayerStaminaChanged);
            _lastValueChangedTime = Time.time;
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerStaminaPayload>(HandlePlayerStaminaChanged);
        }

        private void Update()
        {
            CheckToHideUI();
        }

        private void CheckToHideUI()
        {
            gameObject.SetActive(Time.time < _lastValueChangedTime + _timeOutHideUISec);
        }

        private void HandlePlayerStaminaChanged(PlayerStaminaPayload payload)
        {
            gameObject.SetActive(true);
            
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

            foreach (var img in _imgImageList)
            {
                img.color = color;
            }
        }
    }
}