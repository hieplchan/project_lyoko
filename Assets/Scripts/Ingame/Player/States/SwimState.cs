using StartledSeal.Ingame.Player;
using UnityEngine;
using static StartledSeal.Const;

namespace StartledSeal
{
    public sealed class SwimState : BaseState
    {
        private readonly PlayerVFXController _vfxController;

        public SwimState(PlayerController player) : base(player)
        {
            _vfxController = _player.PlayerVFXControllerComp;
        }
        
        public override void OnEnter()
        {
            _animator.CrossFade(SwimHash, CrossFadeDuration);
            _player.SetStateHash(SwimHash);

            _player.RigidBodyComp.useGravity = false;
            _player.RigidBodyComp.velocity = new Vector3(_player.RigidBodyComp.velocity.x, 0f, _player.RigidBodyComp.velocity.z);
            
            _player.PlayerWeaponControllerComp.DisableUsingItem();

            _vfxController.PlayVFX("Swim");
        }

        public override void FixedUpdate()
        {
            _player.HandleMovement();
        }

        public override void OnExit()
        {
            _player.PlayerWeaponControllerComp.EnableUsingItem();

            _player.RigidBodyComp.useGravity = true;
            _vfxController.StopVFX("Swim");
        }
    }
}