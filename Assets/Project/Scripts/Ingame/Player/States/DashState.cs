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
            _animator.CrossFade(DashHash, CrossFadeDuration);
            _player.SetStateHash(DashHash);
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }
    }
}