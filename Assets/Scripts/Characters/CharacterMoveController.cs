using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] 
    float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] 
    float maxAccelerate = 10f;

    private Vector2 _playerInput;

    private Rigidbody _body;

    private Vector3 _desiredVelocity;
    private Vector3 _velocity;
    private Vector3 _displacement;
    private float _maxSpeedChange;



    private void Awake()
    {
        _body = GetComponent<Rigidbody>();

        _desiredVelocity = Vector3.zero;
        _velocity = Vector3.zero;
        _maxSpeedChange = 0f;
    }

    private void Update()
    {
        _playerInput.x = Input.GetAxis("Horizontal");
        _playerInput.y = Input.GetAxis("Vertical");
        _playerInput = Vector2.ClampMagnitude(_playerInput, 1f);

        _desiredVelocity.x = _playerInput.x;
        _desiredVelocity.z = _playerInput.y;
        _desiredVelocity *= maxSpeed;

        _body.velocity = _velocity;
        _maxSpeedChange = maxAccelerate * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, _desiredVelocity.z, _maxSpeedChange);
        _body.velocity = _velocity;
    }
}