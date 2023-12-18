using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;


[Serializable]
public enum CharacterAnimKey
{
    Idle = 0,
    Run = 1,
    Jump = 2
}

public class CharacterAnimController : SerializedMonoBehaviour
{
    [SerializeField] Character _character;
    [OdinSerialize] Dictionary<CharacterAnimKey, CharacterState> _characterAnimKeyPairs;

    public void SetAnim(CharacterAnimKey key)
    {
        if (_characterAnimKeyPairs.TryGetValue(key, out var value))
            _character.StateMachine.TrySetState(value);
    }

    public void SetDefaultAnim()
    {
        _character.StateMachine.TrySetDefaultState();
    }
}
