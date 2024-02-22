using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public sealed class PetIdleState : PetBaseState
    {
        public PetIdleState(Pet pet, Animator animator) : base(pet, animator)
        {
        }

        public override void OnEnter()
        {
            MLog.Debug("PetIdleState", "OnEnter");
            _animator.CrossFade(IdleHash, CrossDuration);        
        }
    }
}