using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using Sirenix.OdinInspector;
using StartledSeal.Common;
using StartledSeal.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace StartledSeal
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(PlayerDetector))]
    public class Enemy : Entity, IDamageable
    {
        // Event
        public UnityEvent StartChasingEvent;
        public UnityEvent GetHitEvent;
        
        [SerializeField, Self] private NavMeshAgent _agent;
        [SerializeField, Child] private Animator _animator;
        [SerializeField, Self] private PlayerDetector _playerDetector;
        [SerializeField, Self] private HealthComp _healthComp;

        [SerializeField] private float _wanderRadius = 10f;
        [SerializeField] private float _startChasingTimeOffset = 0.5f;
        
        [SerializeField] private float _timeBetweenAttacks = 1f;
        [SerializeField] private float _attackDamage = 1f;

        [SerializeField] private float _getHitTime = 3f;

        [SerializeField] private float _walkSpeed = 1f;
        [SerializeField] private float _runSpeed = 2f;
        
        private StateMachine _stateMachine;
        
        private List<Timer> _timers;
        private CooldownTimer _attackTimer;
        private CooldownTimer _getHitTimer;
        
        private void OnValidate() => this.ValidateRefs();

        private void Start()
        {
            SetupStateMachine();

            SetupTimers();
        }

        private void SetupTimers()
        {
            _attackTimer = new CooldownTimer(_timeBetweenAttacks);
            _getHitTimer = new CooldownTimer(_getHitTime);
            _timers = new List<Timer>(2) { _attackTimer, _getHitTimer };
        }

        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();

            var wanderState = new EnemyWanderState(this, _animator, _agent, _wanderRadius, _walkSpeed);
            var chaseState = new EnemyChaseState(this, _animator, _agent, _playerDetector.Player, _runSpeed, _startChasingTimeOffset);
            var attackState = new EnemyAttackState(this, _animator, _agent, _playerDetector.Player);
            var getHitState = new EnemyGetHitState(this, _animator, _agent);
            var dieState = new EnemyDieState(this, _animator, _agent);
            
            At(wanderState, chaseState, new FuncPredicate(() => _playerDetector.CanDetectPlayer()));
            At(chaseState, wanderState, new FuncPredicate(() => !_playerDetector.CanDetectPlayer()));
            At(chaseState, attackState, new FuncPredicate(() => _playerDetector.CanAttackPlayer()));
            At(attackState, chaseState, new FuncPredicate(() => !_playerDetector.CanAttackPlayer()));
            
            Any(getHitState, new FuncPredicate(() => _getHitTimer.IsRunning));
            At(getHitState, attackState, new FuncPredicate(() => _playerDetector.CanAttackPlayer()));
            At(getHitState, chaseState, new FuncPredicate(() => _playerDetector.CanDetectPlayer()));
            At(getHitState, wanderState, new FuncPredicate(() => !_playerDetector.CanDetectPlayer()));

            Any(dieState, new FuncPredicate(() => _healthComp.IsDead()));
            
            _stateMachine.SetState(wanderState);
        }

        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void Update()
        {
            _stateMachine.Update();
            HandleTimers();
        }
        
        private void HandleTimers()
        {
            foreach (var timer in _timers)
                timer.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }

        public void GetHit(float attackDamage)
        {
            if (_getHitTimer.IsRunning) return;
            
            _healthComp.TakeDamage(_attackDamage);
            _getHitTimer.Start();
        }

        public void Attack()
        {
            if (_attackTimer.IsRunning) return;
            
            _attackTimer.Start();
            _playerDetector.PlayerHealthComp.TakeDamage(_attackDamage);
        }
        
        void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _wanderRadius);
        }

        [Button("TestGetHit")]
        private void TestGetHit() => GetHit(0);

        public UniTask TakeDamage(int damageAmount)
        {
            // MLog.Debug("Enemy", $"TakeDamage {damageAmount}");
            
            GetHit(damageAmount);
            return UniTask.CompletedTask;
        }
    }
}