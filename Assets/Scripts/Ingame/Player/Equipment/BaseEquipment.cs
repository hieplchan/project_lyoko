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

        public virtual UniTask NormalAttack(Animator _animatorComp)
        {
            _animatorComp.CrossFade(_animNormalAttackHash, 0.001f);

            if (_normalAttackAnimSeq != null)
                _normalAttackAnimSeq.Play();
            
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask StartCharging(Animator _animatorComp)
        {
            return UniTask.CompletedTask;
        }
        
        public virtual UniTask ChargedAttack(Animator _animatorComp)
        {
            return UniTask.CompletedTask;
        }
    }
}