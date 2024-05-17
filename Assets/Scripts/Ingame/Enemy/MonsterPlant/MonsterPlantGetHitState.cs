using UnityEngine;

namespace StartledSeal
{
    public class MonsterPlantGetHitState : EnemyBaseState
    {
        private readonly MonsterPlant _monsterPlant;

        public MonsterPlantGetHitState(MonsterPlant monsterPlant, Animator animator) : base(animator)
        {
            _monsterPlant = monsterPlant;
        }

        public override void OnEnter()
        {
            _animator.CrossFade(GetHitHash, CrossDuration);
            _monsterPlant.GetHitEvent?.Invoke();
        }
    }
}