using StartledSeal.Common;
using StartledSeal.Ingame.Player;
using UnityEngine;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class JumpState : BaseState
    {
        public JumpState(PlayerController player) : base(player)
        {
        }

        public override void OnEnter()
        {
            _player.PlayerWeaponControllerComp.DisableUsingItem();
            _player.IsRotationLocked = true;
            
            _animator.CrossFade(JumpHash, CrossFadeDuration);
            _player.SetStateHash(JumpHash);
            _player.PlayerVFXControllerComp.PlayVFX("Jump");
        }

        public override void OnExit()
        {
            _player.PlayerWeaponControllerComp.EnableUsingItem();
            _player.IsRotationLocked = false;
        }

        public override void FixedUpdate()
        {
            _player.HandleJump();
            _player.HandleMovement();
        }
    }
}