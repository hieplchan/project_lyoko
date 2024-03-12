using StartledSeal.Common;
using SuperMaxim.Messaging;
using UnityEngine;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class DeadState : BaseState
    {
        public DeadState(PlayerController player, Animator animator) : base(player, animator)
        {
            _player = player;
            _animator = animator;
        }
        
        public override void OnEnter()
        {
            _animator.CrossFade(DeadHash, CrossFadeDuration);
            _player.SetStateHash(DeadHash);

            Messenger.Default.Publish(new PlayerDeadEventPayload());
        }
    }
}