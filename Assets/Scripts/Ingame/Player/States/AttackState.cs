using UnityEngine;

namespace StartledSeal
{
    public sealed class AttackState : BaseState
    {
        public AttackState(PlayerController player, Animator animator) : base(player, animator)
        {
            _player = player;
            _animator = animator;
        }

        public override void OnEnter()
        {
            _animator.CrossFade(AttackHash, CrossFadeDuration);
            _player.Attack();
            _player.SetStateHash(AttackHash);
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }
    }
}