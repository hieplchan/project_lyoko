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
    [RequireComponent(typeof(PlayerDetector))]
    [RequireComponent(typeof(HealthComp))]
    public abstract class EnemyBase : Entity, IDamageable
    {
        public UnityEvent GetHitEvent;
        public UnityEvent DieEvent;
        
        public Collider ColliderComp => _collider;
        public Rigidbody RigidbodyComp => _rb;
        
        [SerializeField, Self] private Rigidbody _rb;
        [SerializeField, Self] private Collider _collider;
        [SerializeField, Self] private NavMeshAgent _agent;
        [SerializeField, Child] private Animator _animator;
        [SerializeField, Self] private HealthComp _healthComp;
        [SerializeField, Self] private PlayerDetector _playerDetector;
        
        [SerializeField] private float _getHitTime = 3f;
        [SerializeField] private float _getHitImpactForceMultiply = 70f;
        [SerializeField] private float _dieTime = 0.5f;

        private StateMachine _stateMachine;
        private List<Timer> _timers;
        
        private CooldownTimer _getHitTimer;
        
        private void OnValidate() => this.ValidateRefs();
        
        private void Start()
        {
            SetupStateMachine();
            SetupTimers();
        }

        protected virtual void SetupTimers()
        {
            _timers = new List<Timer>();
            
            _getHitTimer = new CooldownTimer(_getHitTime);
            _timers.Add(_getHitTimer);
        }

        protected virtual void SetupStateMachine()
        {
            _stateMachine = new StateMachine();
        }
        
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
        
        private void Update()
        {
            _stateMachine.Update();
            HandleTimers();
        }
        
        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }
        
        private void HandleTimers()
        {
            foreach (var timer in _timers)
                timer.Tick(Time.deltaTime);
        }
        
        public void GetHit(float damageAmount)
        {
            // Wait little bit between gethit multiple times
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