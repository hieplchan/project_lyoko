using StartledSeal.Common;
using UnityEngine;
using static StartledSeal.Utils.Extension.FloatExtensions;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class LocomotionState : BaseState
    {
        private readonly bool _isUsingShield;
        private float _runStaminaConsumptionPerSec;
        
        public LocomotionState(PlayerController player) : base(player)
        {
        }

        public LocomotionState(PlayerController player, float runStaminaConsumptionPerSec) 
            : base(player)
        {
            _runStaminaConsumptionPerSec = runStaminaConsumptionPerSec;
        }

        public override void OnEnter()
        {
            // MLog.Debug("LocomotionState", $"OnEnter IsUsingShield: {_player.PlayerWeaponControllerComp.IsUsingShield}");

            _animator.CrossFade(LocomotionHash, CrossFadeDuration);
            _player.SetStateHash(LocomotionHash);

            _player.PlayerWeaponControllerComp.OnToggleShield(_player.Input.InputActions.Player.Shield.IsPressed());
        }

        public override void Update()
        {
            if (_player.PlayerStaminaComp != null 
                && _player.IsRunning 
                && _player.Movement.magnitude > ZeroF)
            {
                _player.PlayerStaminaComp.ConsumeStamina(_runStaminaConsumptionPerSec * Time.deltaTime);
            }
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }

        public override void OnExit()
        {
            _player.IsRotationLocked = false;
            _player.PlayerWeaponControllerComp.OnToggleShield(false);
        }
    }
}