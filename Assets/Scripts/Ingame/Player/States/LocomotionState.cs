using StartledSeal.Common;
using UnityEngine;
using static StartledSeal.Utils.Extension.FloatExtensions;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class LocomotionState : BaseState
    {
        private readonly bool _isUsingShield;
        private readonly PlayerWeaponController _playerWeaponControllerComp;
        private PlayerStaminaComp _playerStaminaComp;
        private float _runStaminaConsumptionPerSec;
        
        public LocomotionState(PlayerController player, Animator animator) 
            : base(player, animator)
        {
        }

        public LocomotionState(PlayerController player, Animator animator, 
            PlayerStaminaComp playerStaminaComp, float runStaminaConsumptionPerSec) 
            : base(player, animator)
        {
            _playerStaminaComp = playerStaminaComp;
            _runStaminaConsumptionPerSec = runStaminaConsumptionPerSec;
        }

        public override void OnEnter()
        {
            // MLog.Debug("LocomotionState", "OnEnter");

            _animator.CrossFade(LocomotionHash, CrossFadeDuration);
            _player.SetStateHash(LocomotionHash);

            if (_player.IsUsingShield)
            {
                _player.IsRotationLocked = true;
                _player.PlayerWeaponControllerComp.ToggleShield(true);
            }
        }

        public override void Update()
        {
            if (_playerStaminaComp != null 
                && _player.IsRunning 
                && _player.Movement.magnitude > ZeroF)
            {
                _playerStaminaComp.ConsumeStamina(_runStaminaConsumptionPerSec * Time.deltaTime);
            }
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }

        public override void OnExit()
        {
            _player.IsRotationLocked = false;
            _player.PlayerWeaponControllerComp.ToggleShield(false);
        }
    }
}