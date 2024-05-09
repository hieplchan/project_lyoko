using StartledSeal.Ingame.Player;
using UnityEngine;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class AttackState : BaseState
    {
        private readonly PlayerVFXController _vfxController;

        public AttackState(PlayerController player, Animator animator, PlayerVFXController vfxController) : base(player, animator)
        {
            _vfxController = vfxController;
            _player = player;
            _animator = animator;
        }

        public override void OnEnter()
        {
            // _animator.CrossFade(AttackHash, 0.001f);
            // _animator.Play(AttackHash, 0, 0f);
            _player.Attack();
            _player.SetStateHash(AttackHash);

            // _vfxController.RestartVFX("Attack");
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }
    }
}