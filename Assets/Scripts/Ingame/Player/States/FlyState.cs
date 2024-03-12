using SuperMaxim.Messaging;
using UnityEngine;
using static StartledSeal.Const;


namespace StartledSeal
{
    public sealed class FlyState : BaseState
    {
        private float _speed, _flyDrag, _defaultDrag;
        
        public FlyState(PlayerController player, Animator animator, float flyDrag, float defaultDrag) : base(player, animator)
        {
            _flyDrag = flyDrag;
            _defaultDrag = defaultDrag;
        }
        
        public override void OnEnter()
        {
            _animator.CrossFade(FlyHash, CrossFadeDuration);
            _player.SetStateHash(FlyHash);

            _player.RigidBody.drag = _flyDrag;

            Messenger.Default.Publish(new PlayerFlyingPayload()
            {
                IsFlying = true
            });
        }

        public override void FixedUpdate()
        {
            _player.HandleFly();
            _player.HandleMovement();
        }

        public override void OnExit()
        {
            _player.RigidBody.drag = _defaultDrag;
            _player.IsFlying = false;
            
            Messenger.Default.Publish(new PlayerFlyingPayload()
            {
                IsFlying = false
            });
        }
    }
}