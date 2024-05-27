using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public abstract class BaseState : IState
    {
        protected const float CrossFadeDuration = 0.1f;
        
        protected PlayerController _player;
        protected Animator _animator;
        
        protected BaseState(PlayerController player)
        {
            _player = player;
            _animator = player.AnimatorComp;
        }
        
        public virtual void OnEnter()
        {
            // noop
        }

        public virtual void Update()
        {
            // noop
        }

        public virtual void FixedUpdate()
        {
            // noop
        }

        public virtual void OnExit()
        {
            // MLog.Debug("BaseState", "OnExit");
        }
    }
}