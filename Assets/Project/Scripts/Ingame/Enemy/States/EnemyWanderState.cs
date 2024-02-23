using StartledSeal.Common;
using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    public sealed class EnemyWanderState : EnemyBaseState
    {
        private readonly NavMeshAgent _agent;
        private readonly float _wanderRadius;
        private readonly Vector3 _startPoint;
        private float _speed;
        
        public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius, float speed) : base(enemy, animator)
        {
            _agent = agent;
            _wanderRadius = wanderRadius;
            _speed = speed;
            _startPoint = enemy.transform.position;
        }

        public override void OnEnter()
        {
            // MLog.Debug("EnemyWanderState", "OnEnter");
            _animator.CrossFade(WalkHash, CrossDuration);
            _agent.speed = _speed;
        }

        public override void Update()
        {
            if (HasReachedDestination())
            {
                var randomDirection = Random.insideUnitSphere * _wanderRadius;
                randomDirection += _startPoint;
                NavMesh.SamplePosition(randomDirection, out var hit, _wanderRadius, 1);
                var finalPosition = hit.position;
                _agent.SetDestination(finalPosition);
            }
        }

        private bool HasReachedDestination()
        {
            return !_agent.pathPending
                   && _agent.remainingDistance <= _agent.stoppingDistance
                   && (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f);
        }
    }
}