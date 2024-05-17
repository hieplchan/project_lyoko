using StartledSeal.Common;
using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    public sealed class EnemyGetHitState : EnemyBaseState
    {
        private readonly NormalEnemy _normalEnemy;
        private NavMeshAgent _agent;

        public EnemyGetHitState(NormalEnemy normalEnemy, Animator animator, NavMeshAgent agent) : base(animator)
        {
            _normalEnemy = normalEnemy;
            _agent = agent;
        }

        public override void OnEnter()
        {
            MLog.Debug("EnemyGetHitState OnEnter");
            _normalEnemy.GetHitEvent?.Invoke();
            
            _animator.CrossFade(GetHitHash, CrossDuration);
            _agent.isStopped = true;
        }

        public override void OnExit()
        {
            _agent.isStopped = false;
        }
    }
}