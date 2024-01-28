using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public abstract class BaseState : IState
    {
        protected const float CrossFadeDuration = 0.1f;
        
        protected PlayerController player;
        protected Animator animator;
        
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");

        protected BaseState(PlayerController player, Animator animator)
        {
            this.player = player;
            this.animator = animator;
        }
        
        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void OnUpdate()
        {
            // noop
        }

        public virtual void OnFixedUpdate()
        {
            // noop
        }

        public virtual void OnExit()
        {
            MLog.Debug("BaseState", "OnExit");
        }
    }
}