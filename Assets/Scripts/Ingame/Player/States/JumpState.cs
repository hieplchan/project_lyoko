using StartledSeal.Common;
using StartledSeal.Ingame.Player;
using UnityEngine;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class JumpState : BaseState
    {
        private readonly PlayerVFXController _vfxController;

        public JumpState(PlayerController player, Animator animator, PlayerVFXController vfxController) : base(player, animator)
        {
            _vfxController = vfxController;
        }

        public override void OnEnter()
        {
            _player.DisableUsingItem();
            
            _animator.CrossFade(JumpHash, CrossFadeDuration);
            _player.SetStateHash(JumpHash);
            _vfxController.PlayVFX("Jump");
        }

        public override void OnExit()
        {
            _player.EnableUsingItem();
        }

        public override void FixedUpdate()
        {
            _player.HandleJump();
            _player.HandleMovement();
        }
    }
}