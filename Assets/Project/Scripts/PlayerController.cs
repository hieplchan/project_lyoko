using System;
using Cinemachine;
using KBCore.Refs;
using UnityEngine;
using static StartledSeal.FloatExtensions;

namespace StartledSeal
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")] 
        [SerializeField, Self] private Rigidbody _rb;
        [SerializeField, Self] private GroundChecker _groundChecker;
        [SerializeField, Self] private Animator _animator;
        [SerializeField, Anywhere] private CinemachineFreeLook _freeLookCam;
        [SerializeField, Anywhere] private InputReader _input;

        [Header("Settings")] 
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _smoothTime = 0.2f;

        private Transform _mainCamTransform;
        private float _currentSpeed;
        private float _velocity;
        private Vector3 _movement;

        // Animator params
        private static readonly int Speed = Animator.StringToHash("Speed");
        
        private void Awake()
        {
            _mainCamTransform = Camera.main.transform;
            _freeLookCam.Follow = transform;
            _freeLookCam.LookAt = transform;
            // Invoke when observed transform is teleported, adjust _freeLookCam position accordingly
            _freeLookCam.OnTargetObjectWarped(transform, transform.position - _mainCamTransform.position - Vector3.forward);

            _rb.freezeRotation = true;
        }

        private void Start() => _input.EnableplayerActions();

        private void Update()
        {
            _movement = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
            UpdateAnimator();
        }

        private void FixedUpdate()
        {
            // HandleJump();
            HandleMovement();
        }

        private void UpdateAnimator()
        {
            _animator.SetFloat(Speed, _currentSpeed);
        }

        private void HandleMovement()
        {
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(_mainCamTransform.eulerAngles.y, Vector3.up) * _movement;

            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                SmoothSpeed(ZeroF);

                // Reset horizontal velocity for snappy stop
                _rb.velocity = new Vector3(ZeroF, _rb.velocity.y, ZeroF);
            }
        }

        private void HandleRotation(Vector3 adjustedDirection)
        {
            // Adjust rotation to match movement direction
            var targetRotation = Quaternion.LookRotation(adjustedDirection);
            
            // TODO: get rid one of 2 way
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            transform.LookAt(transform.position + adjustedDirection);
        }
        
        private void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // Move the player
            Vector3 velocity = adjustedDirection * (_moveSpeed * Time.fixedDeltaTime);
            _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);
        }
        
        private void SmoothSpeed(float value)
        {
            _currentSpeed = Mathf.SmoothDamp(_currentSpeed, value, ref _velocity, _smoothTime);
        }
    }
}