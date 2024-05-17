using KBCore.Refs;
using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public class MonsterPlant : EnemyBase
    {
        public PlayerDetector PlayerDetectorComp => _playerDetector;
        
        [SerializeField, Self] private PlayerDetector _playerDetector;

        protected override void SetupStateMachine()
        {
            base.SetupStateMachine();

            // state declare
            var idleState = new MonsterPlantIdleState(this, AnimatorComp);
            var attackState = new MonsterPlantAttackState(this, AnimatorComp);
            var getHitState = new MonsterPlantGetHitState(this, AnimatorComp);
            var dieState = new MonsterPlantDieState(this, AnimatorComp, _dieTime);
            
            // transition
            Any(dieState, new FuncPredicate(() => HealthComp.IsDead()));
            Any(idleState, new FuncPredicate(() => !PlayerDetectorComp.CanAttackPlayer() 
                                                   && !_getHitTimer.IsRunning
                                                   && !HealthComp.IsDead()));
            Any(getHitState, new FuncPredicate(() => _getHitTimer.IsRunning && !HealthComp.IsDead()));
            
            At(idleState, attackState, new FuncPredicate(() => PlayerDetectorComp.CanAttackPlayer()));
            At(attackState, idleState, new FuncPredicate(() => !PlayerDetectorComp.CanAttackPlayer()));
            At(getHitState, attackState, new FuncPredicate(() => PlayerDetectorComp.CanAttackPlayer() 
                                                                 && !_getHitTimer.IsRunning));

            StateMachine.SetState(idleState);
        }
        
        public void Attack()
        {
            MLog.Debug("MonsterPlant", "Attack");
        }
    }
}