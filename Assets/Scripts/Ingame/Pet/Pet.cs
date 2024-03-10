using System;
using KBCore.Refs;
using StartledSeal.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Pet : Entity
    {
        [SerializeField, Self] private NavMeshAgent _agent;
        [SerializeField, Child] private Animator _animator;

        [SerializeField] private float _walkToPlayerRadius = 2f;
        [SerializeField] private float _runToPlayerRadius = 4f;
        
        [SerializeField] private float _walkSpeed = 2f;
        [SerializeField] private float _runSpeed = 4f;

        [SerializeField] private float _walkAngularSpeed = 240f;
        [SerializeField] private float _runAngularSpeed = 720f;
        
        private StateMachine _stateMachine;
        private Transform _player;

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag(Const.PlayerTag).transform;
        }

        private void Start()
        {
            SetupStateMachine();
        }

        #region StateMachine
        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();

            var idleState = new PetIdleState(this, _animator);
            var followPlayerRunState = new PetFollowPlayerRunState(this, _animator, _agent, _player, _runSpeed, _runAngularSpeed);
            var followPlayerWalkState = new PetFollowPlayerWalkState(this, _animator, _agent, _player, _walkSpeed, _walkAngularSpeed);

            Any(idleState, new FuncPredicate(() => DistanceToPlayer() < _walkToPlayerRadius));
            
            At(idleState, followPlayerWalkState, new FuncPredicate(CanWalkFollowPlayer));
            At(idleState, followPlayerRunState, new FuncPredicate(CanRunFollowPlayer));
            
            At(followPlayerWalkState, followPlayerRunState, new FuncPredicate(CanRunFollowPlayer));
            At(followPlayerRunState, followPlayerWalkState, new FuncPredicate(CanWalkFollowPlayer));

            
            _stateMachine.SetState(idleState);
        }

        private float DistanceToPlayer()
        {
            var directionToPlayer = _player.position - transform.position;
            return directionToPlayer.magnitude;   
        }
        
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private bool CanWalkFollowPlayer() =>
            DistanceToPlayer() > _walkToPlayerRadius && DistanceToPlayer() < _runToPlayerRadius;

        private bool CanRunFollowPlayer() =>
            DistanceToPlayer() >  _runToPlayerRadius;
        #endregion
        
        private void Update()
        {
            _stateMachine.Update();
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }
        
        void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _walkToPlayerRadius);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _runToPlayerRadius);
        }
    }
}