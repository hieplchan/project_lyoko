using StartledSeal;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CharacterMoveController : MonoBehaviour
{
    [Header("Camera Relative Control")]
    [SerializeField] Transform playerInputSpace = default;

    [Header("Control Setting")]
    [SerializeField, Range(0f, 100f)]   float maxSpeed = 10f;
    [SerializeField, Range(0f, 100f)]   float maxAccelerate = 10f;
    [SerializeField, Range(0f, 90f)]    float maxGroundAngle = 25f;
    [SerializeField, Range(0f, 1f)]     float rotationRate = 0.5f;

    [Header("Jump Setting")]
    [SerializeField, Range(0f, 100f)]   float maxAirAccelerate = 1f; // Harder control when in air
    [SerializeField, Range(0, 5)]       int maxAirJumps = 2; // Dowble jump, tripble jump...    
    [SerializeField, Range(0f, 10f)]    float jumpHeight = 2f; // // Max high character of single jump

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

    // https://catlikecoding.com/unity/tutorials/movement/orbit-camera/#3.2
    private void Update()
    {
        _playerInput.x = Input.GetAxis("Horizontal");
        _playerInput.y = Input.GetAxis("Vertical");
        _playerInput = Vector2.ClampMagnitude(_playerInput, 1f);

        if (playerInputSpace)
        {
            Vector3 forward = playerInputSpace.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 right = playerInputSpace.right;
            right.y = 0;
            right.Normalize();

            var moveDir = forward * _playerInput.y + right * _playerInput.x;
            _desiredVelocity = moveDir * maxSpeed;

            // Character rotation
            if (_playerInput.sqrMagnitude > 0.01)
            {
                var lookDir = Quaternion.LookRotation(moveDir.normalized);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookDir, rotationRate);
            }
        }
        else
        {
            _desiredVelocity = new Vector3(_playerInput.x, 0f, _playerInput.y) * maxSpeed;
        }

        // true until explicity set it false
        _desiredJump |= Input.GetButtonDown("Jump");
    }

    private void FixedUpdate()
    {
        UpdateState();
        AdjustVelocity();

        if (_desiredJump)
        {
            _desiredJump = false;
            Jump();
        }

        _body.velocity = _velocity;

        ClearState();
    }

    private void UpdateState()
    {
        _velocity = _body.velocity;
        if (_onGround)
        {
            _currentAirJump = 0;
            _currentContactNormal.Normalize();
        }
        else
        {            
            _currentContactNormal = Vector3.up;
        }
    }
    private void ClearState()
    {
        _onGround = false;
        _currentContactNormal = Vector3.zero;
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

    // https://catlikecoding.com/unity/tutorials/movement/physics/#3.5
    private void AdjustVelocity()
    {
        // Project XZ plant to current contact plane
        Vector3 xAxis = ProjectOnContactPlane(Vector3.right).normalized;
        Vector3 zAxis = ProjectOnContactPlane(Vector3.forward).normalized;

        // Project velocity to new current plane
        float currentX = Vector3.Dot(_velocity, xAxis);
        float currentZ = Vector3.Dot(_velocity, zAxis);

        _currentAccelerate = _onGround ? maxAccelerate : maxAirAccelerate;
        _maxSpeedChange = _currentAccelerate * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentX, _desiredVelocity.x, _maxSpeedChange);
        float newZ = Mathf.MoveTowards(currentZ, _desiredVelocity.z, _maxSpeedChange);
        _velocity += xAxis * (newX - currentX) + zAxis * (newZ - currentZ);
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
                _currentContactNormal += normal;
            }
        }
    }

    // vector = x + normal.dot(normal, vector)
    // => x = vector - normal.dot(normal, vector)
    Vector3 ProjectOnContactPlane(Vector3 vector)
    {
        return vector - _currentContactNormal * Vector3.Dot(_currentContactNormal, vector);
    }
}