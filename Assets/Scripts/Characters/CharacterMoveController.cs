using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMoveController : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)] float maxAccelerate = 10f;
    [Tooltip("Hard to control when in air")]
    [SerializeField, Range(0f, 100f)] float maxAirAccelerate = 1f;
    [Tooltip("Dowble jump, tripble jump...")]
    [SerializeField, Range(0, 5)] int maxAirJumps = 2;
    [Tooltip("How high character can do in single jump")]
    [SerializeField, Range(0f, 10f)] float jumpHeight = 2f;
    [SerializeField, Range(0f, 90f)] float maxGroundAngle = 25f;

    private Rigidbody _body;

    private Vector2 _playerInput;
    private Vector3 _desiredVelocity;
    private bool _desiredJump;
    private float _minGroundDotProduct;

    private float _currentAccelerate;
    private Vector3 _velocity;
    private float _maxSpeedChange;
    private int _currentAirJump;
    private float _jumpSpeed;
    // While in air/double jump, not have contact normal => use Vector3.up
    private Vector3 _currentContactNormal;

    public bool OnGround => _onGround;
    private bool _onGround;

    private void OnValidate()
    {
        _minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private void Awake()
    {
        _body = GetComponent<Rigidbody>();
        OnValidate();
    }

    private void OnCollisionEnter(Collision collision) => EvaluateCollision(collision);
    private void OnCollisionStay(Collision collision) => EvaluateCollision(collision);

    private void Update()
    {
        _playerInput.x = Input.GetAxis("Horizontal");
        _playerInput.y = Input.GetAxis("Vertical");
        _playerInput = Vector2.ClampMagnitude(_playerInput, 1f);

        _desiredVelocity.x = _playerInput.x;
        _desiredVelocity.y = 0f;
        _desiredVelocity.z = _playerInput.y;
        _desiredVelocity *= maxSpeed;

        // true until explicity set it false
        _desiredJump |= Input.GetButtonDown("Jump");
    }

    private void FixedUpdate()
    {
        UpdateState();

        _currentAccelerate = _onGround ? maxAccelerate : maxAirAccelerate;
        _maxSpeedChange = _currentAccelerate * Time.deltaTime;
        _velocity.x = Mathf.MoveTowards(_velocity.x, _desiredVelocity.x, _maxSpeedChange);
        _velocity.z = Mathf.MoveTowards(_velocity.z, _desiredVelocity.z, _maxSpeedChange);

        if (_desiredJump)
        {
            _desiredJump = false;
            Jump();
        }

        _body.velocity = _velocity;

        _onGround = false;
    }

    private void UpdateState()
    {
        _velocity = _body.velocity;
        if (_onGround)
        {
            _currentAirJump = 0;
        }
        else
        {
            
            _currentContactNormal = Vector3.up;
        }
    }

    // https://catlikecoding.com/unity/tutorials/movement/physics/#2.2
    // https://catlikecoding.com/unity/tutorials/movement/physics/#2.6
    // https://catlikecoding.com/unity/tutorials/movement/physics/#3.4
    private void Jump()
    {
        if (_onGround || _currentAirJump < maxAirJumps)
        {
            _currentAirJump++;
            _jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);

            // limit jump initial velocity
            float alignSpeed = Vector3.Dot(_velocity, _currentContactNormal);
            if (alignSpeed > 0f)
                _jumpSpeed = Mathf.Max(_jumpSpeed - alignSpeed, 0f);

            _velocity += _currentContactNormal * _jumpSpeed;
        }
    }

    // https://catlikecoding.com/unity/tutorials/movement/physics/#3.3
    private void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= _minGroundDotProduct)
            {
                _onGround = true;
                _currentContactNormal = normal;
            }
        }
    }
}