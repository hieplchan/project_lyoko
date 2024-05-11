using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    public class EnemyDieState : EnemyBaseState
    {
        private NavMeshAgent _agent;

        public EnemyDieState(Enemy enemy, Animator animator, NavMeshAgent agent) : base(enemy, animator)
        {
            _agent = agent;
        }
        
        public override void OnEnter()
        {
            _enemy.GetHitEvent?.Invoke();
            
            _animator.CrossFade(DieHash, CrossDuration);
            _agent.isStopped = true;
        }

        public override void OnExit()
        {
            _agent.isStopped = false;
        }
    }
}