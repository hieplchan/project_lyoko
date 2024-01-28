using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity
    {
        [SerializeField, Self] private NavMeshAgent _agent;
        [SerializeField, Child] private Animator _animator;
        [SerializeField, Self] private PlayerDetector _playerDetector;

        [SerializeField] private float _wanderRadius = 10f;

        private StateMachine _stateMachine;
        
        private void OnValidate() => this.ValidateRefs();

        private void Start()
        {
            _stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, _animator, _agent, _wanderRadius);
            var chaseState = new EnemyChaseState(this, _animator, _agent, _playerDetector.Player);
            
            At(wanderState, chaseState, new FuncPredicate(() => _playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !_playerDetector.CanDetectPlayer()));
            
            _stateMachine.SetState(wanderState);
        }
        
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void Update()
        {
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }
    }
}