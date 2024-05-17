using StartledSeal.Common;
using StartledSeal.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    public sealed class EnemyChaseState : EnemyBaseState
    {
        private readonly NormalEnemy _normalEnemy;
        private NavMeshAgent _agent;
        private Transform _player;
        private float _speed;

        private CooldownTimer _startChasingCountDownTimer;

        public EnemyChaseState(NormalEnemy normalEnemy, Animator animator, NavMeshAgent agent, Transform player, float speed, float startChasingTimeOffset) 
            : base(animator)
        {
            _normalEnemy = normalEnemy;
            _agent = agent;
            _player = player;
            _speed = speed;

            _startChasingCountDownTimer = new CooldownTimer(startChasingTimeOffset);
        }

        public override void OnEnter()
        {
            // MLog.Debug("EnemyChaseState", "OnEnter");
            _animator.CrossFade(ChaseHash, CrossDuration);
            _agent.speed = _speed;
            
            _normalEnemy.StartChasingEvent?.Invoke();

            _agent.isStopped = true;
            _startChasingCountDownTimer.Start();
            _startChasingCountDownTimer.OnTimerStop += () =>
            {
                _agent.isStopped = false;
            };
        }

        public override void Update()
        {
            _startChasingCountDownTimer.Tick(Time.deltaTime);
            _agent.SetDestination(_player.position);
        }
    }
}