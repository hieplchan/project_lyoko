using StartledSeal.Common;
using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    public sealed class EnemyChaseState : EnemyBaseState
    {
        private NavMeshAgent _agent;
        private Transform _player;
        private float _speed;

        public EnemyChaseState(Enemy enemy, Animator animator, NavMeshAgent agent, Transform player, float speed) : base(enemy, animator)
        {
            _agent = agent;
            _player = player;
            _speed = speed;
        }

        public override void OnEnter()
        {
            MLog.Debug("EnemyChaseState", "OnEnter");
            _animator.CrossFade(RunHash, CrossDuration);
            _agent.speed = _speed;
        }

        public override void Update()
        {
            _agent.SetDestination(_player.position);
        }
    }
}