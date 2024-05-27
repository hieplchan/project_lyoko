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
        [field: SerializeField] public float AttackCoolDown = 0.5f;

        [SerializeField] private AnimationSequencerController _normalAttackAnimSeq;
        [SerializeField] private AnimationSequencerController _startChargingAnimSeq;
        [SerializeField] private AnimationSequencerController _chargedAttackAnimSeq;
        
        [SerializeField] private string NormalAttackAnimState;
        [SerializeField] private string ChargingAnimState;
        [SerializeField] private string ChargedAttackAnimState;

        protected int _animNormalAttackHash;
        protected int _animStartChargingHash;
        protected int _animChargedAttackHash;

        private void Awake()
        {
            _animNormalAttackHash = Animator.StringToHash(NormalAttackAnimState);
            _animStartChargingHash = Animator.StringToHash(ChargingAnimState);
            _animChargedAttackHash = Animator.StringToHash(ChargedAttackAnimState);
        }

        public abstract bool IsUsable();

        public virtual UniTask NormalAttack(PlayerController playerController)
        {
            playerController.AnimatorComp.Play(_animNormalAttackHash, 0, 0f);

            if (_normalAttackAnimSeq != null)
                _normalAttackAnimSeq.Play();
            
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
            
            return UniTask.CompletedTask;        
        }
    }
}