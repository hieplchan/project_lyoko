using System.Collections.Generic;
using KBCore.Refs;
using Sirenix.OdinInspector;
using StartledSeal.Common;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.UI;

namespace StartledSeal.Project.Scripts.UI
{
    public class PlayerBatterySlider : ValidatedMonoBehaviour
    {
        
        [SerializeField, Self] private CanvasGroup _canvasGroup;
        [SerializeField] private CanvasGroup _iconImageCanvasGroup;
        
        [SerializeField] private float _timeOutHideUISec = 0.5f;

        [SerializeField] private bool _isAutoHideEnable;
        
        [SerializeField] private List<Slider> _sliderList;

        [SerializeField] private List<Image> _imgBGList;
        [SerializeField] private List<UIOutline> _outlineBGList;

        [SerializeField] private Gradient _gradientColor;
        private float _lastValueChangedTime;

        private void Awake()
        {
            Messenger.Default.Subscribe<PlayerBatteryPayload>(HandlePlayerBatteryChanged);
            Messenger.Default.Subscribe<PlayerDeadEventPayload>(HandlePlayerDeadEvent);

            _lastValueChangedTime = Time.time;
            _iconImageCanvasGroup.alpha = 0;
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerBatteryPayload>(HandlePlayerBatteryChanged);
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

        private void HandlePlayerBatteryChanged(PlayerBatteryPayload payload)
        {
            _canvasGroup.alpha = 1;
            
            _lastValueChangedTime = Time.time;
            
            var value = payload.CurrentBattery / payload.MaxBattery;
            var color = _gradientColor.Evaluate(1 - value);

            _iconImageCanvasGroup.alpha = 1 - value > 0f ? 1 : 0;
                
            foreach (var slider in _sliderList)
            {
                slider.value = value;
            }

            ChangeColor(color);
        }
        
        private void HandlePlayerDeadEvent(PlayerDeadEventPayload payload)
        {
            MLog.Debug("HandlePlayerDeadEvent");
            Messenger.Default.Unsubscribe<PlayerBatteryPayload>(HandlePlayerBatteryChanged);
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
            ChangeColor(_gradientColor.Evaluate(0f));
        }
        #endif
    }
}