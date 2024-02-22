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
        
        public EnemyWanderState(Enemy enemy, Animator animator, NavMeshAgent agent, float wanderRadius) : base(enemy, animator)
        {
            _agent = agent;
            _wanderRadius = wanderRadius;
            _startPoint = enemy.transform.position;
        }

        public override void OnEnter()
        {
            // MLog.Debug("EnemyWanderState", "OnEnter");
            _animator.CrossFade(WalkHash, CrossDuration);
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