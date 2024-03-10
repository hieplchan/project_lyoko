using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public abstract class BaseState : IState
    {
        protected const float CrossFadeDuration = 0.1f;
        
        protected PlayerController _player;
        protected Animator _animator;
        
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");
        protected static readonly int DashHash = Animator.StringToHash("Dash");
        protected static readonly int AttackHash = Animator.StringToHash("Attack");
        protected static readonly int DeadHash = Animator.StringToHash("Dead");
        
        protected BaseState(PlayerController player, Animator animator)
        {
            _player = player;
            _animator = animator;
        }
        
        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void Update()
        {
            // noop
        }

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnExit()
        {
            // MLog.Debug("BaseState", "OnExit");
        }
    }
}