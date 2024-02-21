using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public sealed class LocomotionState : BaseState
    {
        public LocomotionState(PlayerController player, Animator animator) : base(player, animator)
        {
        }

        public override void OnEnter()
        {
            _animator.CrossFade(LocomotionHash, CrossFadeDuration);
            _player.SetStateHash(LocomotionHash);
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }
    }
}