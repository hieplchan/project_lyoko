using StartledSeal.Ingame.Player;
using UnityEngine;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class SwimState : BaseState
    {
        private readonly PlayerVFXController _vfxController;

        public SwimState(PlayerController player, Animator animator, PlayerVFXController vfxController) : base(player, animator)
        {
            _vfxController = vfxController;
        }
        
        public override void OnEnter()
        {
            _animator.CrossFade(SwimHash, CrossFadeDuration);
            _player.SetStateHash(SwimHash);

            _player.RigidBody.useGravity = false;
            _player.RigidBody.velocity = new Vector3(_player.RigidBody.velocity.x, 0f, _player.RigidBody.velocity.z);

            _vfxController.PlayVFX("Swim");
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }

        public override void OnExit()
        {
            _player.RigidBody.useGravity = true;
            _vfxController.StopVFX("Swim");
        }
    }
}