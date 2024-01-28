using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public sealed class JumpState : BaseState
    {
        public JumpState(PlayerController player, Animator animator) : base(player, animator)
        {
        }

        public override void OnEnter()
        {
            MLog.Debug("JumpState", "OnEnter");
            _animator.CrossFade(JumpHash, CrossFadeDuration);
        }

        public override void FixedUpdate()
        {
            _player.HandleJump();
            _player.HandleMovement();
        }
    }
}