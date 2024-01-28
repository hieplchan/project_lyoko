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
            MLog.Debug("LocomotionState", "OnEnter");
            animator.CrossFade(LocomotionHash, CrossFadeDuration);
        }

        public override void OnFixedUpdate()
        {
            player.HandleMovement();
        }
    }
}