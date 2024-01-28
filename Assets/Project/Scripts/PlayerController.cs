using System;
using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using StartledSeal.Utils;
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

        [Header("Movement Settings")] 
        [SerializeField] private float _moveSpeed = 6f;
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _smoothTime = 0.2f;

        [Header("Jump Settings")] 
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _jumpCoolDown = 0f;
        [SerializeField] private float _jumpGravityMultiplier = 3f; // falling faster
        [SerializeField] private float _jumpLaunchPoint = 0.9f; // max jump force at begining

        private Transform _mainCamTransform;
        
        private float _currentSpeed;
        private float _velocity;
        private float _jumpVelocity;

        private Vector3 _movement;

        // Timers
        private List<Timer> _timers;
        private CooldownTimer _jumpTimer; // how long user hold jump btn
        private CooldownTimer _jumpCooldownTimer; // wait to next jump

        // State Machine
        private StateMachine _stateMachine;
        
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

            SetupTimers();
            SetupStateMachine();
        }

        private void SetupTimers()
        {
            _jumpTimer = new CooldownTimer(_jumpDuration);
            _jumpCooldownTimer = new CooldownTimer(_jumpCoolDown);
            _timers = new List<Timer>(2) { _jumpTimer, _jumpCooldownTimer };

            _jumpTimer.OnTimerStart += () => _jumpVelocity = _jumpForce;
            _jumpTimer.OnTimerStop += () => _jumpCooldownTimer.Start();
        }

        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();
            var locomotionState = new LocomotionState(this, _animator);
            var jumpState = new JumpState(this, _animator);

            At(locomotionState, jumpState, new FuncPredicate(() => _jumpTimer.IsRunning));
            At(jumpState, locomotionState, new FuncPredicate(() => _groundChecker.IsGrounded && !_jumpTimer.IsRunning));

            _stateMachine.SetState(locomotionState);
        }

        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private void Start() => _input.EnableplayerActions();

        private void OnEnable()
        {
            _input.Jump += OnJump;
        }

        private void OnDisable()
        {
            _input.Jump -= OnJump;
        }
        
        private void Update()
        {
            _movement = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
            _stateMachine.Update();
            
            HandleTimers();
            UpdateAnimator();
        }

        private void HandleTimers()
        {
            foreach (var timer in _timers)
                timer.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }
        
        private void OnJump(bool performed)
        {
            if (performed && !_jumpCooldownTimer.IsRunning &&
                !_jumpTimer.IsRunning && // not jump when jumping
                _groundChecker.IsGrounded) // only jump when on ground
            {
                _jumpTimer.Start();
            }
            else if (!performed && _jumpTimer.IsRunning)
            {
                _jumpTimer.Stop();
            }
        }

        public void HandleJump()
        {
            // If not jumping and grounded, keep jump velocity at 0
            if (!_jumpTimer.IsRunning && _groundChecker.IsGrounded)
            {
                _jumpVelocity = ZeroF;
                _jumpTimer.Stop();
                return;
            }
            
            // falling
            if (!_jumpTimer.IsRunning)
            {
                _jumpVelocity += Physics.gravity.y * _jumpGravityMultiplier * Time.fixedDeltaTime;
            }

            // Apply velocity
            _rb.velocity = new Vector3(_rb.velocity.x, _jumpVelocity, _rb.velocity.z);
        }

        public void HandleMovement()
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
        
        private void UpdateAnimator()
        {
            _animator.SetFloat(Speed, _currentSpeed);
        }

    }
}