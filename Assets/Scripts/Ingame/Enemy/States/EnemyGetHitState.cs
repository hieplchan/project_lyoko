using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    public sealed class EnemyGetHitState : EnemyBaseState
    {
        private NavMeshAgent _agent;

        public EnemyGetHitState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
        {
            _agent = agent;
        }

        public override void OnEnter()
        {
            _enemy.GetHitEvent?.Invoke();
            
            _animator.CrossFade(GetHitHash, CrossDuration);
            _agent.isStopped = true;
        }

        public override void OnExit()
        {
            _agent.isStopped = false;
        }
    }
}