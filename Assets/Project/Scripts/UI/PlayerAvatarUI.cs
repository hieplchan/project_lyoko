using BrunoMikoski.AnimationSequencer;
using KBCore.Refs;
using Sirenix.OdinInspector;
using SuperMaxim.Messaging;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerAvatarUI : ValidatedMonoBehaviour
    {
        [SerializeField] private UIOutline _outline;

        [Header("Heart Beat Effect")] 
        [SerializeField] private float _minAnimSpeed = 0.6f;
        [SerializeField] private float _maxAnimSpeed = 1.6f;
        [SerializeField, Child] private AnimationSequencerController _heartbeatAnimController;

        private void Awake()
        {
            Messenger.Default.Subscribe<PlayerDeadEventPayload>(HandlePlayerDeadEvent);
            SetHeartbeatSpeed(_minAnimSpeed);
        }

        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerDeadEventPayload>(HandlePlayerDeadEvent);
        }
        
        private void HandlePlayerDeadEvent(PlayerDeadEventPayload payload)
        {
            _outline.color = Color.red;
            SetHeartbeatSpeed(0f);
        }

        [Button]
        private void SetHeartbeatSpeed(float timeScale)
        {
            _heartbeatAnimController.PlayingSequence.timeScale = timeScale;
        }
    }
}