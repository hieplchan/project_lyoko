using StartledSeal.Common;
using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    public sealed class EnemyChaseState : EnemyBaseState
    {
        private NavMeshAgent _agent;
        private Transform _player;
        
        public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player) : base(enemy, animator)
        {
            _agent = agent;
            _player = player;
        }

        public override void OnEnter()
        {
            MLog.Debug("EnemyChaseState", "OnEnter");
            _animator.CrossFade(RunHash, CrossDuration);
        }

        public override void Update()
        {
            _agent.SetDestination(_player.position);
        }
    }
}