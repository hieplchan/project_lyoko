using UnityEngine;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class DashState : BaseState
    {
        public DashState(PlayerController player) : base(player)
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