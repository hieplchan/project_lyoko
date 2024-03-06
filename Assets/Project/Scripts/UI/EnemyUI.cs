using System;
using BrunoMikoski.AnimationSequencer;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace StartledSeal.Project.Scripts.UI
{
    public class EnemyUI : ValidatedMonoBehaviour
    {
        [SerializeField, Parent] private Enemy _enemy;

        [SerializeField] private AnimationSequencerController _startChasingNotiAnim;
        [SerializeField] private ParticleSystem _getHitParticleSystem;

        private void Awake()
        {
            _enemy.StartChasingEvent.AddListener(HandleStartChasingEvent);
            _enemy.GetHitEvent.AddListener(HandleGetHitEvent);
        }

        private void OnDestroy()
        {
            _enemy.StartChasingEvent.RemoveListener(HandleStartChasingEvent);
            _enemy.GetHitEvent.RemoveListener(HandleGetHitEvent);
        }

        private void HandleStartChasingEvent() => _startChasingNotiAnim.Play();
        private void HandleGetHitEvent() => _getHitParticleSystem.Play();
    }
}