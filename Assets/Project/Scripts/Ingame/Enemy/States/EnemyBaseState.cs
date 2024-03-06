using UnityEngine;

namespace StartledSeal
{
    public abstract class EnemyBaseState : IState
    {
        protected const float CrossDuration = 0.1f;
        
        protected readonly Enemy _enemy;
        protected readonly Animator _animator;
        
        protected static readonly int IdleHash = Animator.StringToHash("Idle");
        protected static readonly int RunHash = Animator.StringToHash("Run");
        protected static readonly int WalkHash = Animator.StringToHash("Walk");
        protected static readonly int AttackHash = Animator.StringToHash("Attack");
        protected static readonly int DieHash = Animator.StringToHash("Die");
        protected static readonly int GetHit = Animator.StringToHash("GetHit");
        
        protected EnemyBaseState(Enemy enemy, Animator animator)
        {
            _enemy = enemy;
            _animator = animator;
        }

        public virtual void OnEnter()
        {
            // no op
        }

        public virtual void Update()
        {
            // no op
        }

        public virtual void FixedUpdate()
        {
            // no op
        }

        public virtual void OnExit()
        {
            // no op
        }
    }
}