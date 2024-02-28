using Sirenix.OdinInspector;
using System;
using UnityEngine;


[Serializable]
public enum CharacterAnimKey
{
    Idle = 0,
    Run = 1,
    Jump = 2
}

[Serializable]
public class CharacterAnimKeyState : SerializedDictionary<CharacterAnimKey, CharacterState>
{
}


public sealed class CharacterAnimController : SerializedMonoBehaviour
{
    [SerializeField] Character _character;
    [SerializeField] CharacterAnimKeyState _characterAnimKeyPairs;

    public void SetAnim(CharacterAnimKey key)
    {
        if (_characterAnimKeyPairs.TryGetValue(key, out var value))
            _character.StateMachine.TryResetState(value);
    }

    public void SetDefaultAnim()
    {
        _character.StateMachine.TrySetDefaultState();
    }
}
