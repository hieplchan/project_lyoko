using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public class MonsterPlantAttackState : EnemyBaseState
    {
        private readonly MonsterPlant _monsterPlant;

        public MonsterPlantAttackState(MonsterPlant monsterPlant, Animator animator) : base(animator)
        {
            _monsterPlant = monsterPlant;
        }

        public override void OnEnter()
        {
            _animator.Play(AttackHash, 0, 0f);
        }

        public override void Update()
        {
            _monsterPlant.gameObject.transform.LookAt(_monsterPlant.PlayerDetectorComp.Player);
        }
    }
}