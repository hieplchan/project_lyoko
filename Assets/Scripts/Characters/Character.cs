using Animancer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Character : MonoBehaviour
{
    [SerializeField] private Transform _cameraPoint;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private AnimancerComponent _animancer;
    [SerializeField] private CharacterState.StateMachine _stateMachine;
    [SerializeField] private CharacterMoveController _characterMoveController;
    [SerializeField] private CharacterSoundController _characterSoundController;
    [SerializeField] private CharacterAnimController _characterAnimController;

    public Transform CameraPoint => _cameraPoint;
    public Rigidbody RigidBody => _rigidBody;
    public AnimancerComponent Animancer => _animancer;
    public CharacterState.StateMachine StateMachine => _stateMachine;
    public CharacterMoveController MoveController => _characterMoveController;
    public CharacterSoundController SoundController => _characterSoundController;
    public CharacterAnimController AnimController => _characterAnimController;

    private void Awake()
    {
        StateMachine.InitializeAfterDeserialize();
    }
}
