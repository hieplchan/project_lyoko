using UnityEngine;

namespace StartledSeal
{
    public class PetBaseState : IState
    {
        protected const float CrossDuration = 0.3f;
        
        protected readonly Pet _pet;
        protected readonly Animator _animator;
        
        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int RunHash = Animator.StringToHash("Run");
        protected static readonly int WalkHash = Animator.StringToHash("Walk");

        protected PetBaseState(Pet pet, Animator animator)
        {
            _pet = pet;
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
            // noop
        }
    }
}