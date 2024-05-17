using StartledSeal.Common;
using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    public sealed class EnemyAttackState : EnemyBaseState
    {
        private readonly NormalEnemy _normalEnemy;
        private NavMeshAgent _agent;
        private Transform _player;
        
        public EnemyAttackState(NormalEnemy normalEnemy, Animator animator, NavMeshAgent agent, Transform player) : base(animator)
        {
            _normalEnemy = normalEnemy;
            _agent = agent;
            _player = player;
        }
        
        public override void OnEnter()
        {
            // MLog.Debug("EnemyAttackState", "OnEnter");
            _animator.CrossFade(AttackHash, CrossDuration);
            _agent.speed = 0.1f;
        }

        public override void Update()
        {
            _agent.SetDestination(_player.position);
            _normalEnemy.Attack();
        }
    }
}