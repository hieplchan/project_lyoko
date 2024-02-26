using Animancer;
using Animancer.Examples.StateMachines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu(Strings.ExamplesMenuPrefix + "CharacterState - Action")]
public class ActionState : CharacterState
{
    public override CharacterStatePriority Priority => CharacterStatePriority.Medium;
    public override bool CanInterruptSelf => true;

    private void Awake()
    {
        _animation.Events.OnEnd = Character.StateMachine.ForceSetDefaultState;
    }
}
