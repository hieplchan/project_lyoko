using StartledSeal.Common;
using StartledSeal.Utils;
using SuperMaxim.Messaging;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace StartledSeal
{
    public class EnemyDieState : EnemyBaseState
    {
        private readonly NormalEnemy _normalEnemy;
        private NavMeshAgent _agent;
        private CooldownTimer _cooldownTimer;

        public EnemyDieState(NormalEnemy normalEnemy, Animator animator, NavMeshAgent agent, float dieTime) : base(animator)
        {
            _normalEnemy = normalEnemy;
            _agent = agent;

            _cooldownTimer = new CooldownTimer(dieTime);
        }
        
        public override void OnEnter()
        {
            MLog.Debug("EnemyDieState OnEnter");
            
            _animator.CrossFade(DieHash, CrossDuration);
            _agent.isStopped = true;
            
            _normalEnemy.DieEvent?.Invoke();
            _normalEnemy.ColliderComp.enabled = false;
            
            _cooldownTimer.Start();
            _cooldownTimer.OnTimerStop += () =>
            {
                _normalEnemy.DestroyAfterDie();
                RequestSpawnCollectible(_normalEnemy.transform);
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
        
        private void RequestSpawnCollectible(Transform transform)
        {
            var payload = new SpawnCollectibleRequest()
            {
                spawnTransform = transform
            };
            Messenger.Default.Publish(payload);
        }
    }
}