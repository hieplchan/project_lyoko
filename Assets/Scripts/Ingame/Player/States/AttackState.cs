using StartledSeal.Common;
using StartledSeal.Ingame.Player;
using UnityEngine;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class AttackState : BaseState
    {
        public AttackState(PlayerController player) : base(player)
        {
        }

        public override void OnEnter()
        {
            EnableUpperBodyAnimMask(true);
            
            // MLog.Debug("AttackState", "OnEnter");

            // _animator.CrossFade(AttackHash, 0.001f);
            // _animator.Play(AttackHash, 0, 0f);
            // _player.Attack();
            // _player.SetStateHash(AttackHash);

            // _vfxController.RestartVFX("Attack");
        }

        public override void OnExit()
        {
            EnableUpperBodyAnimMask(false);
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }

        private void EnableUpperBodyAnimMask(bool isEnable)
        {
            _player.AnimatorComp.SetLayerWeight(UpperBodyAnimLayer, isEnable? 1 : 0);
        }
    }
}