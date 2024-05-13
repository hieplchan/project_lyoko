using StartledSeal.Common;
using StartledSeal.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace StartledSeal
{
    public class EnemyDieState : EnemyBaseState
    {
        private readonly Enemy _enemy;
        private NavMeshAgent _agent;
        private CooldownTimer _cooldownTimer;

        public EnemyDieState(Enemy enemy, Animator animator, NavMeshAgent agent, float dieTime) : base(enemy, animator)
        {
            _enemy = enemy;
            _agent = agent;

            _cooldownTimer = new CooldownTimer(dieTime);
        }
        
        public override void OnEnter()
        {
            MLog.Debug("EnemyDieState OnEnter");
            
            _animator.CrossFade(DieHash, CrossDuration);
            _agent.isStopped = true;
            
            _enemy.DieEvent?.Invoke();
            
            _cooldownTimer.Start();
            _cooldownTimer.OnTimerStop += () =>
            {
                _enemy.DestroyAfterDie();
            };
        }

        public override void Update()
        {
            _cooldownTimer.Tick(Time.deltaTime);
        }

        public override void OnExit()
        {
            _agent.isStopped = false;
        }
    }
}