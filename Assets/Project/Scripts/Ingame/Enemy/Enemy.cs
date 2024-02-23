using System;
using KBCore.Refs;
using StartledSeal.Common;
using StartledSeal.Utils;
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
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private float _attackDamage = 1f;

        [SerializeField] private float _walkSpeed = 1f;
        [SerializeField] private float _runSpeed = 2f;
        
        private StateMachine _stateMachine;
        
        private CooldownTimer _attackTimer;
        
        private void OnValidate() => this.ValidateRefs();

        private void Start()
        {
            _stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, _animator, _agent, _wanderRadius, _walkSpeed);
            var chaseState = new EnemyChaseState(this, _animator, _agent, _playerDetector.Player, _runSpeed);
            var attackState = new EnemyAttackState(this, _animator, _agent, _playerDetector.Player);
            
            At(wanderState, chaseState, new FuncPredicate(() => _playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !_playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => _playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !_playerDetector.CanAttackPlayer()));

            _stateMachine.SetState(wanderState);

            _attackTimer = new CooldownTimer(_timeBetweenAttacks);
        }
        
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void Update()
        {
            _stateMachine.Update();
            _attackTimer.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        public void Attack()
        {
            if (_attackTimer.IsRunning) return;
            
            _attackTimer.Start();
            _playerDetector.PlayerHealth.TakeDamage(_attackDamage);
        }
        
        void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _wanderRadius);
        }
    }
}