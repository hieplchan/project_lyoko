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
        ChargedAttackState = 3
    }
    
    public abstract class BaseEquipment : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] protected PlayerWeaponController _weaponController;
        [SerializeField] protected bool _isShowGizmos;

        [Header("Normal Attack")]
        [SerializeField] private string NormalAttackAnimState;
        [SerializeField] private AnimationSequencerController _normalAttackAnimSeq;

        [Header("Charging Phase")] 
        [SerializeField] public float StartChargingTime = 0.5f;
        [SerializeField] public string ChargingAnimState;
        [SerializeField] private AnimationSequencerController _chargingAnimSeq;
        
        [Header("Charged Attack")]
        [SerializeField] public float ChargedAttackDuration = 0.3f;
        [field: SerializeField] public float ChargedAttackTime = 0.3f;
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
            MLog.Debug("BaseEquipment", $"Use {active}");
            
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
                    case EquipmentState.NotBeingUsed:
                        break;
                    case EquipmentState.NormalAttackState:
                        if (Time.time < _lastUsedCheckpoint + StartChargingTime)
                        {
                            StopUsing();
                        }
                        break;
                    case EquipmentState.ChargingState:
                        StopUsing();
                        break;
                    case EquipmentState.ChargedAttackState:
                        break;
                }
            }
        }
        
        public virtual void Update()
        {
            switch (CurrentState)
            {
                case EquipmentState.NotBeingUsed:
                    break;
                case EquipmentState.NormalAttackState:
                    if (Time.time > _lastUsedCheckpoint + StartChargingTime)
                    {
                        StartCharging();
                    }
                    break;
                case EquipmentState.ChargingState:
                    break;
                case EquipmentState.ChargedAttackState:
                    break;
            }
        }

        public virtual void NormalAttack()
        {
            CurrentState = EquipmentState.NormalAttackState;

            MarkLastUsedTime();

            PlayAnimAndVFX(_animNormalAttackHash, _normalAttackAnimSeq).Forget();
        }
        
        public void StartCharging()
        {
            MLog.Debug("BaseEquipment", "StartCharging");
            CurrentState = EquipmentState.ChargingState;
            _player.IsRotationLocked = true;
            _player.IsForcedWalking = true;
            
            PlayAnimAndVFX(_animChargingHash, _chargingAnimSeq).Forget();
        }
        
        public virtual void ChargedAttack()
        {
            CurrentState = EquipmentState.ChargedAttackState;
            
            MarkLastUsedTime();
            
            PlayAnimAndVFX(_animChargedAttackHash, _chargedAttackAnimSeq).Forget();
        }

        private UniTask PlayAnimAndVFX(int animHash, AnimationSequencerController animSeq)
        {
            _player.AnimatorComp.Play(animHash, Const.UpperBodyAnimLayer, 0f);
            animSeq?.Play();
            return UniTask.CompletedTask;
        }

        private void MarkLastUsedTime() => _lastUsedCheckpoint = Time.time;

        public void StopUsing()
        {
            CurrentState = EquipmentState.NotBeingUsed;
            _player.IsRotationLocked = false;
            _player.IsForcedWalking = false;
            
            _normalAttackAnimSeq?.Kill(false);
            _chargingAnimSeq?.Kill(false);
            _chargedAttackAnimSeq?.Kill(false);
        }
    }
}