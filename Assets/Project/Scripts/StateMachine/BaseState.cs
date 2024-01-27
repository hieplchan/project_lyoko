using UnityEngine;

namespace StartledSeal
{
    public abstract class BaseState : IState
    {
        protected PlayerController playerController;
        protected Animator animator;
        
        protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
        protected static readonly int JumpHash = Animator.StringToHash("Jump");

        protected BaseState(PlayerController playerController, Animator animator)
        {
            this.playerController = playerController;
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
            // noop
        }
    }
}