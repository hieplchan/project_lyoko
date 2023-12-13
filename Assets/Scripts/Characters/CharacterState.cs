using Animancer;
using Animancer.Examples.StateMachines;
using Animancer.FSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState : StateBehaviour
{
    [Serializable]
    public class StateMachine : StateMachine<CharacterState>.WithDefault { }

    [SerializeField] private Character _character;
    [SerializeField] protected ClipTransition _animation;

    public Character Character => _character;

    protected virtual void OnEnable()
    {
        Character.Animancer.Play(_animation);
    }

    public virtual CharacterStatePriority Priority => CharacterStatePriority.Low;
    public virtual bool CanInterruptSelf => false;
    public override bool CanExitState
    {
        get
        {
            var nextState = _character.StateMachine.NextState;
            if (nextState == this)
                return CanInterruptSelf;
            else if (Priority == CharacterStatePriority.Low)
                return true;
            else
                return nextState.Priority > Priority;
        }
    }

#if UNITY_EDITOR
    //protected override void OnValidate()
    //{
    //    base.OnValidate();
    //    // Lazy to manually assign Character to each component
    //    gameObject.GetComponentInParentOrChildren(ref _character);
    //}
#endif
}
