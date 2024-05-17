using StartledSeal.Utils;
using SuperMaxim.Messaging;
using UnityEngine;

namespace StartledSeal
{
    public class MonsterPlantDieState : EnemyBaseState
    {
        private readonly MonsterPlant _monsterPlant;
        private CooldownTimer _cooldownTimer;

        public MonsterPlantDieState(MonsterPlant monsterPlant, Animator animator, float dieTime) : base(animator)
        {
            _monsterPlant = monsterPlant;
            _cooldownTimer = new CooldownTimer(dieTime);
        }

        public override void OnEnter()
        {
            _animator.CrossFade(DieHash, CrossDuration);
            
            _monsterPlant.DieEvent?.Invoke();
            _monsterPlant.ColliderComp.enabled = false;
            
            _cooldownTimer.Start();
            _cooldownTimer.OnTimerStop += () =>
            {
                _monsterPlant.DestroyAfterDie();
                RequestSpawnCollectible(_monsterPlant.transform);
            };
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