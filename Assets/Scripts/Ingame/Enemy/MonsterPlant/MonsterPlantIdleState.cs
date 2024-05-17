using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public class MonsterPlantIdleState : EnemyBaseState
    {
        private readonly MonsterPlant _monsterPlant;

        public MonsterPlantIdleState(MonsterPlant monsterPlant, Animator animator) : base(animator)
        {
            _monsterPlant = monsterPlant;
        }

        public override void OnEnter()
        {
            _animator.CrossFade(IdleHash, CrossDuration);
        }
    }
}