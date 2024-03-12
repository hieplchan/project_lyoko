using UnityEngine;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class SwimState : BaseState
    {
        public SwimState(PlayerController player, Animator animator) : base(player, animator)
        {
        }
        
        public override void OnEnter()
        {
            _animator.CrossFade(SwimHash, CrossFadeDuration);
            _player.SetStateHash(SwimHash);

            _player.RigidBody.useGravity = false;
            _player.RigidBody.velocity = new Vector3(_player.RigidBody.velocity.x, 0f, _player.RigidBody.velocity.z);
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }

        public override void OnExit()
        {
            _player.RigidBody.useGravity = true;
        }
    }
}