using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using StartledSeal.Common;
using UnityEngine;
using UnityEngine.Serialization;

namespace StartledSeal
{
    public enum EquipmentState
    {
        NotBeingUsed = 0,
        NormalAttackState = 1,
        ChargingState = 2,
        ChargeDoneState = 3,
        ChargedAttackState = 4
    }
    
    public abstract class BaseEquipment : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] protected PlayerWeaponController _weaponController;
        [SerializeField] protected bool _isShowGizmos;

        [Header("Normal Attack")] 
        [SerializeField]private float NormalAttackDuration = 0.15f;
        [SerializeField] private string NormalAttackAnimState;
        [SerializeField] private AnimationSequencerController _normalAttackAnimSeq;

        [Header("Charging Phase")] 
        [SerializeField] public float StartChargingTime = 0.25f;
        [field: SerializeField] public float ChargingDuration = 0.5f;
        [SerializeField] public string ChargingAnimState;
        [SerializeField] private AnimationSequencerController _chargingAnimSeq;
        [SerializeField] private AnimationSequencerController _chargingDoneAnimSeq;
        
        [Header("Charged Attack")]
        [SerializeField] public float ChargedAttackDuration = 0.3f;
        [SerializeField] private string ChargedAttackAnimState;
        [SerializeField] private AnimationSequencerController _chargedAttackAnimSeq;
        
        protected PlayerController _player => _weaponController.PlayerControllerComp;

        public EquipmentState CurrentState { get; private set; }
        public virtual bool IsUsable() => true;
        
        protected int _animNormalAttackHash;
        protected int _animChargingHash;
        protected int _animChargedAttackHash;
        
        private float _lastUsedCheckpoint;

        private void Awake()
        {
            _animNormalAttackHash = Animator.StringToHash(NormalAttackAnimState);
            _animChargingHash = Animator.StringToHash(ChargingAnimState);
            _animChargedAttackHash = Animator.StringToHash(ChargedAttackAnimState);
            
            MarkLastUsedTime();
        }

        public virtual void Use(bool active)
        {
            if (!IsUsable()) return;

            if (active)
            {
                if (CurrentState == EquipmentState.NotBeingUsed)
                    NormalAttack();
            }
            else
            {
                switch (CurrentState)
                {
                    case EquipmentState.NormalAttackState:
                        if (Time.time < _lastUsedCheckpoint + StartChargingTime)
                            StopUsing();
                        break;
                    case EquipmentState.ChargingState:
                        StopUsing();
                        break;
                    case EquipmentState.ChargeDoneState:
                        ChargedAttack();
                        break;
                }
            }
        }
        
        public virtual void Update()
        {
            switch (CurrentState)
            {
                case EquipmentState.NormalAttackState:
                    if (Time.time > _lastUsedCheckpoint + StartChargingTime)
                        StartCharging();
                    break;
                case EquipmentState.ChargingState:
                    if (Time.time > _lastUsedCheckpoint + StartChargingTime + ChargingDuration)
                        ChargeDone();
                    break;
                case EquipmentState.ChargedAttackState:
                    if (Time.time > _lastUsedCheckpoint + ChargedAttackDuration)
                        StopUsing();
                    break;
            }
        }

        public virtual void NormalAttack()
        {
            CurrentState = EquipmentState.NormalAttackState;
            MarkLastUsedTime();

            EnableUpperBodyAnimMask(false);
            PlayAnimAndVFX(0, _animNormalAttackHash, _normalAttackAnimSeq).Forget();
        }
        
        public virtual void StartCharging()
        {
            CurrentState = EquipmentState.ChargingState;
            _player.IsRotationLocked = true;
            _player.IsForcedWalking = true;
            
            EnableUpperBodyAnimMask(true);
            PlayAnimAndVFX(Const.UpperBodyAnimLayer, _animChargingHash, _chargingAnimSeq).Forget();
        }

        public virtual void ChargeDone()
        {
            CurrentState = EquipmentState.ChargeDoneState;
            _chargingDoneAnimSeq?.Play();
        }
        
        public virtual void ChargedAttack()
        {
            CurrentState = EquipmentState.ChargedAttackState;
            MarkLastUsedTime();
            
            EnableUpperBodyAnimMask(false);
            PlayAnimAndVFX(0, _animChargedAttackHash, _chargedAttackAnimSeq).Forget();
        }

        private UniTask PlayAnimAndVFX(int animLayer, int animHash, AnimationSequencerController animSeq)
        {
            _player.AnimatorComp.Play(animHash, animLayer, 0f);
            animSeq?.Play();
            return UniTask.CompletedTask;
        }

        private void MarkLastUsedTime() => _lastUsedCheckpoint = Time.time;

        public virtual void StopUsing()
        {
            CurrentState = EquipmentState.NotBeingUsed;
            _player.IsRotationLocked = false;
            _player.IsForcedWalking = false;
            EnableUpperBodyAnimMask(false);
        }
        
        private void EnableUpperBodyAnimMask(bool isEnable)
        {
            _player.AnimatorComp.SetLayerWeight(Const.UpperBodyAnimLayer, isEnable? 1 : 0);
        }
    }
}