using UnityEngine;
using static StartledSeal.Const;

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
            _animator.CrossFade(AttackHash, 0.001f);
            // _animator.Play(AttackHash, 0, 0f);
            _player.Attack();
            _player.SetStateHash(AttackHash);
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }
    }
}