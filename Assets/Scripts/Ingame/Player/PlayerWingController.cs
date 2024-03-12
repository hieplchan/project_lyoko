using System;
using System.Collections;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using SuperMaxim.Messaging;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerWingController : MonoBehaviour
    {
        [SerializeField] private Transform _wingPos;
        [SerializeField] private AnimationSequencerController _animIn;
        [SerializeField] private AnimationSequencerController _animOut;

        private void Awake()
        {
            Messenger.Default.Subscribe<PlayerFlyingPayload>(HandleFlyingMessage);
        }
        
        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerFlyingPayload>(HandleFlyingMessage);
        }

        private void HandleFlyingMessage(PlayerFlyingPayload payload)
        {
            if (payload.IsFlying)
                _animIn.Play();
            else
                _animOut.Play();
        }
    }
}