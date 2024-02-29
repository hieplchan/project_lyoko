using BrunoMikoski.AnimationSequencer;
using Coffee.UIEffects;
using KBCore.Refs;
using Sirenix.OdinInspector;
using StartledSeal.Common;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace StartledSeal
{
    public class PlayerAvatarUI : ValidatedMonoBehaviour
    {
        [SerializeField] private UIOutline _outline;
        [SerializeField, Child] private UIDissolve _heartImageDissolve;
        
        [Header("Heart Beat Effect")] 
        [SerializeField] private Gradient _outlineBGGradient;
        [SerializeField] private float _minAnimSpeed = 0.6f;
        [SerializeField] private float _maxAnimSpeed = 1.6f;
        [SerializeField, Child] private AnimationSequencerController _heartbeatAnimController;

        private void Awake()
        {
            Messenger.Default.Subscribe<PlayerDeadEventPayload>(HandlePlayerDeadEvent);
            Messenger.Default.Subscribe<NearbyEnemyDistancePayload>(HandleNearbyEnemyDistanceMessage);
            SetHeartbeatSpeed(_minAnimSpeed);

            _outline.color = _outlineBGGradient.Evaluate(0f);
            _heartImageDissolve.effectFactor = 0.2f;
        }

        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerDeadEventPayload>(HandlePlayerDeadEvent);
            Messenger.Default.Unsubscribe<NearbyEnemyDistancePayload>(HandleNearbyEnemyDistanceMessage);
        }
        
        private void HandlePlayerDeadEvent(PlayerDeadEventPayload payload)
        {
            Messenger.Default.Unsubscribe<NearbyEnemyDistancePayload>(HandleNearbyEnemyDistanceMessage);

            _outline.color = _outlineBGGradient.Evaluate(1f);
            SetHeartbeatSpeed(0f);
        }
        
        private void HandleNearbyEnemyDistanceMessage(NearbyEnemyDistancePayload payload)
        {
            _outline.color = _outlineBGGradient.Evaluate(payload.WarningRatio);
            SetHeartbeatSpeed(Mathf.Lerp(_minAnimSpeed, _maxAnimSpeed, payload.WarningRatio));
            _heartImageDissolve.effectFactor = Mathf.Lerp(0.2f, 0.5f, payload.WarningRatio);
        }

        [Button]
        private void SetHeartbeatSpeed(float timeScale)
        {
            _heartbeatAnimController.PlayingSequence.timeScale = timeScale;
            MLog.Debug($"SetHeartbeatSpeed {timeScale}");
        }
    }
}