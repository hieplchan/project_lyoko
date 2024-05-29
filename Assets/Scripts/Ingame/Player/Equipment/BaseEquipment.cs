using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
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
        [SerializeField] public float NormalAttackDuration = 0.15f;
        [SerializeField] private string NormalAttackAnimState;
        [SerializeField] private AnimationSequencerController _normalAttackAnimSeq;

        [Header("Charging Phase")] 
        [SerializeField] public float StartChargingTime = 1f;
        [SerializeField] public float ChargingDuration = 0.15f;
        [SerializeField] public string ChargingAnimState;
        [SerializeField] private AnimationSequencerController _startChargingAnimSeq;
        
        [Header("Charged Attack")]
        [SerializeField] public float ChargedAttackDuration = 0.3f;
        [field: SerializeField] public float ChargedAttackTime = 0.3f;
        [SerializeField] private string ChargedAttackAnimState;
        [SerializeField] private AnimationSequencerController _chargedAttackAnimSeq;
        
        protected PlayerController _player => _weaponController.PlayerControllerComp;

        public EquipmentState CurrentState { get; private set; }
        public virtual bool IsUsable() => true;
        
        protected int _animNormalAttackHash;
        protected int _animStartChargingHash;
        protected int _animChargedAttackHash;
        
        private float _lastUsedCheckpoint;

        private void Awake()
        {
            _animNormalAttackHash = Animator.StringToHash(NormalAttackAnimState);
            _animStartChargingHash = Animator.StringToHash(ChargingAnimState);
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
                
            }
        }
        
        public virtual void Update()
        {
            switch (CurrentState)
            {
                case EquipmentState.NotBeingUsed:
                    break;
                case EquipmentState.NormalAttackState:
                    if (Time.time > _lastUsedCheckpoint + NormalAttackDuration)
                    {
                        CurrentState = EquipmentState.NotBeingUsed;
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
        
        public virtual UniTask StartCharging(PlayerController playerController)
        {
            return UniTask.CompletedTask;
        }
        
        public virtual void ChargedAttack()
        {
            CurrentState = EquipmentState.ChargedAttackState;
            
            MarkLastUsedTime();
            
            PlayAnimAndVFX(_animChargedAttackHash, _chargedAttackAnimSeq).Forget();
        }

        private UniTask PlayAnimAndVFX(int animHash, AnimationSequencerController animSeq)
        {
            _player.AnimatorComp.Play(animHash, 0, 0f);

            if (animSeq != null)
                animSeq.Play();

            return UniTask.CompletedTask;
        }

        private void MarkLastUsedTime() => _lastUsedCheckpoint = Time.time;
    }
}