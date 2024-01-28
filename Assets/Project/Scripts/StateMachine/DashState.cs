using UnityEngine;

namespace StartledSeal
{
    public sealed class DashState : BaseState
    {
        public DashState(PlayerController player, Animator animator) : base(player, animator)
        {
        }

        public override void OnEnter()
        {
            animator.CrossFade(DashHash, CrossFadeDuration);
        }

        public override void OnFixedUpdate()
        {
            player.HandleMovement();
        }
    }
}