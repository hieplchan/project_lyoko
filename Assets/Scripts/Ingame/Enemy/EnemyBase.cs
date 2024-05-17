using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExternalPropertyAttributes;
using KBCore.Refs;
using StartledSeal.Common;
using StartledSeal.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace StartledSeal
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(HealthComp))]
    public abstract class EnemyBase : Entity, IDamageable
    {
        public UnityEvent GetHitEvent;
        public UnityEvent DieEvent;
        
        public Collider ColliderComp => _collider;
        public Rigidbody RigidbodyComp => _rb;
        public Animator AnimatorComp => _animator;
        public CooldownTimer AttackCooldownTimer => _attackCooldownTimer;
        public HealthComp HealthComp => _healthComp;

        [SerializeField, Self] private Rigidbody _rb;
        [SerializeField, Self] private Collider _collider;
        [SerializeField, Self] private NavMeshAgent _agent;
        [SerializeField, Child] private Animator _animator;
        [SerializeField, Self] private HealthComp _healthComp;

        [SerializeField] private float _attackCooldownTime = 1.1f;
        [SerializeField] private float _getHitTime = 0.5f;
        [SerializeField] private float _getHitImpactForceMultiply = 70f;
        [SerializeField] protected float _dieTime = 0.5f;

        protected StateMachine StateMachine;
        
        protected List<Timer> Timers;
        protected CooldownTimer _getHitTimer;
        protected CooldownTimer _attackCooldownTimer;
        
        private void OnValidate() => this.ValidateRefs();
        
        private void Start()
        {
            SetupStateMachine();
            SetupTimers();
        }

        protected virtual void SetupTimers()
        {
            Timers = new List<Timer>();
            
            _getHitTimer = new CooldownTimer(_getHitTime);
            _attackCooldownTimer = new CooldownTimer(_attackCooldownTime);
            
            Timers.Add(_getHitTimer);
            Timers.Add(_attackCooldownTimer);
        }

        protected virtual void SetupStateMachine()
        {
            StateMachine = new StateMachine();
        }
        
        protected void At(IState from, IState to, IPredicate condition) => StateMachine.AddTransition(from, to, condition);
        protected void Any(IState to, IPredicate condition) => StateMachine.AddAnyTransition(to, condition);
        
        private void Update()
        {
            StateMachine.Update();
            HandleTimers();
        }
        
        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }
        
        private void HandleTimers()
        {
            foreach (var timer in Timers)
                timer.Tick(Time.deltaTime);
        }
        
        public void GetHit(float damageAmount)
        {
            // Wait a little bit between get hit multiple times
            if (_getHitTimer.IsRunning) return;
            
            _healthComp.TakeDamage(damageAmount);
            _getHitTimer.Start();
        }

        public UniTask TakeDamage(int damageAmount, Transform impactObject)
        {
            GetHit(damageAmount);
            ApplyFallbackForce(damageAmount, impactObject);
            
            return UniTask.CompletedTask;
        }
        
        private void ApplyFallbackForce(int damageAmount, Transform impactObject)
        {
            Vector3 impactVector = transform.position - impactObject.position;
            impactVector.y = 0;
            _rb.AddForce(impactVector * damageAmount * _getHitImpactForceMultiply);
        }

        public void DestroyAfterDie()
        {
            Destroy(gameObject);
        }
    }
}