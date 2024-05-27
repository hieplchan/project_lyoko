using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;

namespace StartledSeal
{
    public abstract class BaseEquipment : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] protected PlayerWeaponController _weaponController;
        [SerializeField] private float AttackCoolDown = 0.5f;
        [SerializeField] public float NormalAttackTime = 0.15f;
        [SerializeField] public float ChargedAttackTime = 0.3f;
        [field: SerializeField] public float ChargedAttackThresholdSec = 0.3f;

        [SerializeField] private AnimationSequencerController _normalAttackAnimSeq;
        [SerializeField] private AnimationSequencerController _startChargingAnimSeq;
        [SerializeField] private AnimationSequencerController _chargedAttackAnimSeq;
        
        [SerializeField] private string NormalAttackAnimState;
        [SerializeField] private string ChargingAnimState;
        [SerializeField] private string ChargedAttackAnimState;
 
        protected int _animNormalAttackHash;
        protected int _animStartChargingHash;
        protected int _animChargedAttackHash;

        private float _lastUsedCheckpoint;

        private void Awake()
        {
            _animNormalAttackHash = Animator.StringToHash(NormalAttackAnimState);
            _animStartChargingHash = Animator.StringToHash(ChargingAnimState);
            _animChargedAttackHash = Animator.StringToHash(ChargedAttackAnimState);
            _lastUsedCheckpoint = Time.time;
        }

        public virtual bool IsUsable() => Time.time > _lastUsedCheckpoint + AttackCoolDown;

        public virtual UniTask NormalAttack(PlayerController playerController)
        {
            playerController.AnimatorComp.Play(_animNormalAttackHash, 0, 0f);

            if (_normalAttackAnimSeq != null)
                _normalAttackAnimSeq.Play();

            _lastUsedCheckpoint = Time.time;
            
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask StartCharging(PlayerController playerController)
        {
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask ChargedAttack(PlayerController playerController)
        {
            playerController.AnimatorComp.Play(_animChargedAttackHash, 0, 0f);
            
            if (_chargedAttackAnimSeq != null)
                _chargedAttackAnimSeq.Play();
            
            _lastUsedCheckpoint = Time.time;
            
            return UniTask.CompletedTask;        
        }
    }
}