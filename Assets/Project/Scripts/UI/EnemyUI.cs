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

        private void Awake()
        {
            _enemy.StartChasingEvent.AddListener(HandleStartChasingEvent);
        }

        private void OnDestroy()
        {
            _enemy.StartChasingEvent.RemoveListener(HandleStartChasingEvent);
        }

        private void HandleStartChasingEvent() => _startChasingNotiAnim.Play();
    }
}