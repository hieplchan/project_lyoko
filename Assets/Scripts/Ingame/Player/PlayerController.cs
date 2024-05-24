using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using StartledSeal.Ingame.Player;
using StartledSeal.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using static StartledSeal.Utils.Extension.FloatExtensions;
using static StartledSeal.Const;

namespace StartledSeal
{
    public partial class PlayerController : ValidatedMonoBehaviour, IDamageable
    {
        [Header("Event")] 
        public UnityEvent GetHitEvent;
        
        [field: SerializeField, Child] public Rigidbody RigidBodyComp { get; private set; }
        [field: SerializeField, Child] public Animator AnimatorComp { get; private set; }
        [field: SerializeField, Child] public PlayerVFXController PlayerVFXControllerComp { get; private set; }
        [field: SerializeField, Child] public GroundChecker GroundCheckerComp { get; private set; }
        [field: SerializeField, Child] public WaterChecker WaterCheckerComp { get; private set; }
        
        [SerializeField, Anywhere] private CinemachineFreeLook _freeLookCam;
        [SerializeField, Anywhere] private InputReader _input;
        
        [Header("Movement Settings")] 
        [SerializeField] private float _runSpeed = 200f;
        [SerializeField] private float _walkSpeed = 130f;
        [SerializeField] private float _usingShieldSpeed = 150f;
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _smoothTime = 0.2f;
        [SerializeField] private float _runStaminaConsumptionPerSec = 10;
        [SerializeField] private float _runStaminaThreshold = 50;
        
        [Header("Jump Settings")] 
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _jumpGravityMultiplier = 3f; // falling faster
        
        [Header("Fly Setting")]
        [SerializeField] private float _flySpeed = 400f;
        [SerializeField] private float _flyDrag = 6f;
        
        [Header("Swim Setting")]
        [SerializeField] private float _swimSpeed = 150f;

        [Header("Dash Setting")] 
        [SerializeField] private float _dashForce = 10f;
        [SerializeField] private float _dashDuration = 1f;
        [SerializeField] private float _dashCoolDown = 2f;
        
        private Transform _mainCamTransform;
        
        // Movement
        public bool IsRunning;
        public bool IsFlying;
        public bool IsRotationLocked;
        
        // private float _currentSpeed;
        // private float _velocity;
        private float _jumpVelocity;
        private float _dashVelocity = 1f;

        public Vector3 Movement { get; private set; }

        private void Awake()
        {
            _mainCamTransform = Camera.main.transform;
            _freeLookCam.Follow = transform;
            _freeLookCam.LookAt = transform;
            // Invoke when observed transform is teleported, adjust _freeLookCam position accordingly
            _freeLookCam.OnTargetObjectWarped(transform, transform.position - _mainCamTransform.position - Vector3.forward);

            RigidBodyComp.freezeRotation = true;

            PlayerHealthComp.SetTag(Const.PlayerTag);
            
            SetupTimers();
            SetupStateMachine();
        }


        private void Start() => _input.EnableplayerActions();

        private void OnEnable()
        {
            _input.Jump += OnJump;
            _input.Dash += OnDash;
            _input.Run += OnRun;

            EnableUsingItem();

            PlayerStaminaComp.RunOutStamina += OnRunOutOfStamina;
        }

        private void OnDisable()
        {
            _input.Jump -= OnJump;
            _input.Dash -= OnDash;
            _input.Run -= OnRun;

            DisableUsingItem();
            
            PlayerStaminaComp.RunOutStamina -= OnRunOutOfStamina;
        }
        
        private void Update()
        {
            Movement = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
            _stateMachine.Update();
            
            HandleTimers();
            UpdateAnimator();
        }
        
        private void FixedUpdate()
        {
            _stateMachine.FixedUpdate();
        }
        
        private void OnJump(bool performed)
        {
            if (!performed) return;
            if (GroundCheckerComp.IsGrounded) // only jump when on ground
            {
                if (_jumpTimer.IsRunning) return; // not jump when jumping
                _jumpVelocity = _jumpForce;
                _jumpTimer.Start();
            }
            else
            {
                // handle in-air
                // IsFlying = !IsFlying;
            }
        }

        private void OnRun(bool performed)
        {
            if (performed && PlayerStaminaComp.CurrentStamina > _runStaminaThreshold)
                IsRunning = true;
            else
                IsRunning = false;
        }

        private void OnDash(bool performed)
        {
            if (performed && !_dashCooldownTimer.IsRunning &&
                !_dashTimer.IsRunning &&
                GroundCheckerComp.IsGrounded)
            {
                _dashTimer.Start();
            }
            else if (!performed && !_dashTimer.IsRunning)
            {
                _dashTimer.Stop();
            }
        }

        public void HandleJump()
        {
            // If not jumping and grounded, keep jump velocity at 0
            if (!_jumpTimer.IsRunning && GroundCheckerComp.IsGrounded)
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
            RigidBodyComp.velocity = new Vector3(RigidBodyComp.velocity.x, _jumpVelocity, RigidBodyComp.velocity.z);
        }

        public void HandleMovement()
        {
            // Rotate movement direction to match camera rotation
            var adjustedDirection = Quaternion.AngleAxis(_mainCamTransform.eulerAngles.y, Vector3.up) * Movement;

            if (adjustedDirection.magnitude > ZeroF
                && _currentStateHash != AttackHash)
            {
                if (!IsRotationLocked)
                    HandleRotation(adjustedDirection);
                
                HandleHorizontalMovement(adjustedDirection);
                // SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                // SmoothSpeed(ZeroF);

                // Reset horizontal velocity for snappy stop
                RigidBodyComp.velocity = new Vector3(ZeroF, RigidBodyComp.velocity.y, ZeroF);
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
            var moveSpeed = IsRunning ? _runSpeed : _walkSpeed;
            if (IsUsingShield)
                moveSpeed = _usingShieldSpeed;
            moveSpeed = _currentStateHash.Equals(FlyHash) ? _flySpeed : moveSpeed;
            moveSpeed = _currentStateHash.Equals(SwimHash) ? _swimSpeed : moveSpeed;

            // Move the player
            Vector3 velocity = adjustedDirection * (moveSpeed * _dashVelocity * Time.fixedDeltaTime);
            RigidBodyComp.velocity = new Vector3(velocity.x, RigidBodyComp.velocity.y, velocity.z);
        }
        
        // private void SmoothSpeed(float value)
        // {
        //     _currentSpeed = Mathf.SmoothDamp(_currentSpeed, value, ref _velocity, _smoothTime);
        // }
        
        private void UpdateAnimator()
        {
            // _animator.SetFloat(Speed, _currentSpeed);
            AnimatorComp.SetFloat(Speed, RigidBodyComp.velocity.magnitude);
        }
    }
}