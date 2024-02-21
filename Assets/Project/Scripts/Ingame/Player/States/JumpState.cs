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
            _animator.CrossFade(JumpHash, CrossFadeDuration);
            _player.SetStateHash(JumpHash);
        }

        public override void FixedUpdate()
        {
            _player.HandleJump();
            _player.HandleMovement();
        }
    }
}