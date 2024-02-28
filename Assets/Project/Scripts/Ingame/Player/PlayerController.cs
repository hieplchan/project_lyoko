using System;
using System.Collections.Generic;
using Cinemachine;
using KBCore.Refs;
using StartledSeal.Utils;
using UnityEngine;
using static StartledSeal.Utils.Extension.FloatExtensions;

namespace StartledSeal
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")] 
        [SerializeField, Self] private Rigidbody _rb;
        [SerializeField, Self] private GroundChecker _groundChecker;
        [SerializeField, Child] private Animator _animator;
        [SerializeField, Anywhere] private CinemachineFreeLook _freeLookCam;
        [SerializeField, Anywhere] private InputReader _input;
        
        [SerializeField, Self] private PlayerStaminaComp _playerStaminaComp;
        [SerializeField, Self] private HealthComp _playerHealthComp;
        
        [Header("Movement Settings")] 
        [SerializeField] private float _runSpeed = 200f;
        [SerializeField] private float _walkSpeed = 130f;
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _smoothTime = 0.2f;
        [SerializeField] private float _runStaminaConsumptionPerSec = 10;
        [SerializeField] private float _runStaminaThreshold = 50;
        
        [Header("Jump Settings")] 
        [SerializeField] private float _jumpForce = 10f;
        [SerializeField] private float _jumpDuration = 0.5f;
        [SerializeField] private float _jumpCoolDown = 0f;
        [SerializeField] private float _jumpGravityMultiplier = 3f; // falling faster

        [Header("Dash Setting")] 
        [SerializeField] private float _dashForce = 10f;
        [SerializeField] private float _dashDuration = 1f;
        [SerializeField] private float _dashCoolDown = 2f;

        [Header("Attack Setting")] 
        [SerializeField] private float _attackCoolDown = 0.5f;
        [SerializeField] private float _attackDistance = 1f;
        [SerializeField] private int _attackDamage = 10;
        
        private Transform _mainCamTransform;
        
        // Movement
        public bool IsRunning;
        
        // private float _currentSpeed;
        // private float _velocity;
        private float _jumpVelocity;
        private float _dashVelocity = 1f;

        public Vector3 Movement { get; private set; }

        // Timers
        private List<Timer> _timers;
        
        private CooldownTimer _jumpTimer; // how long user hold jump btn
        private CooldownTimer _jumpCooldownTimer; // wait to next jump

        private CooldownTimer _dashTimer;
        private CooldownTimer _dashCooldownTimer;

        private CooldownTimer _attackCooldownTimer;

        // State Machine
        private StateMachine _stateMachine;
        private int _currentStateHash;
        
        // Animator params
        private static readonly int Speed = Animator.StringToHash("Speed");
        private static readonly int AttackHash = Animator.StringToHash("Attack");

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

            _dashTimer = new CooldownTimer(_dashDuration);
            _dashCooldownTimer = new CooldownTimer(_dashCoolDown);

            _attackCooldownTimer = new CooldownTimer(_attackCoolDown);
            
            _timers = new List<Timer>(5) { _jumpTimer, _jumpCooldownTimer, _dashTimer, _dashCooldownTimer, _attackCooldownTimer };

            _jumpTimer.OnTimerStart += () => _jumpVelocity = _jumpForce;
            _jumpTimer.OnTimerStop += () => _jumpCooldownTimer.Start();

            _dashTimer.OnTimerStart += () => _dashVelocity = _dashForce;
            _dashTimer.OnTimerStop += () =>
            {
                _dashCooldownTimer.Start();
                _dashVelocity = 1f;
            };
        }

        #region State Machine
        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();
            
            // declare state
            var locomotionState = new LocomotionState(this, _animator, _playerStaminaComp, _runStaminaConsumptionPerSec);
            var jumpState = new JumpState(this, _animator);
            var dashState = new DashState(this, _animator);
            var attackState = new AttackState(this, _animator);
            var deadState = new DeadState(this, _animator);
            
            // define transition
            At(locomotionState, jumpState, new FuncPredicate(() => _jumpTimer.IsRunning));
            At(locomotionState, dashState, new FuncPredicate(() => _dashTimer.IsRunning));
            
            At(locomotionState, attackState, new FuncPredicate(() => _attackCooldownTimer.IsRunning));
            At(attackState, locomotionState, new FuncPredicate(() => !_attackCooldownTimer.IsRunning));
            
            At(dashState, jumpState, new FuncPredicate(() => !_dashTimer.IsRunning && _jumpTimer.IsRunning));
            At(jumpState, dashState, new FuncPredicate(() => _dashTimer.IsRunning && !_jumpTimer.IsRunning));
            
            At(dashState, attackState, new FuncPredicate(() => !_dashTimer.IsRunning && _attackCooldownTimer.IsRunning));
            At(attackState, dashState, new FuncPredicate(() => _dashTimer.IsRunning && !_attackCooldownTimer.IsRunning));
            
            At(locomotionState, deadState, new FuncPredicate(() => _playerHealthComp.IsDead()));
            
            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));

            // set init state
            _stateMachine.SetState(locomotionState);
        }
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);

        private bool ReturnToLocomotionState()
        {
            return _groundChecker.IsGrounded
                   && !_jumpTimer.IsRunning
                   && !_dashTimer.IsRunning
                   && !_attackCooldownTimer.IsRunning
                   && !_playerHealthComp.IsDead();
        }

        public void SetStateHash(int stateHash)
        {
            _currentStateHash = stateHash;
        }
        #endregion

        private void Start() => _input.EnableplayerActions();

        private void OnEnable()
        {
            _input.Jump += OnJump;
            _input.Dash += OnDash;
            _input.Run += OnRun;
            // _input.Attack += OnAttack;

            _playerStaminaComp.RunOutStamina += OnRunOutOfStamina;
        }

        private void OnDisable()
        {
            _input.Jump -= OnJump;
            _input.Dash -= OnDash;
            _input.Run -= OnRun;
            // _input.Attack -= OnAttack;
            
            _playerStaminaComp.RunOutStamina -= OnRunOutOfStamina;
        }
        
        private void OnRunOutOfStamina() => IsRunning = false;

        private void Update()
        {
            Movement = new Vector3(_input.Direction.x, 0f, _input.Direction.y);
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

        private void OnRun(bool performed)
        {
            if (performed && _playerStaminaComp.CurrentStamina > _runStaminaThreshold)
                IsRunning = true;
            else
                IsRunning = false;
        }

        private void OnDash(bool performed)
        {
            if (performed && !_dashCooldownTimer.IsRunning &&
                !_dashTimer.IsRunning &&
                _groundChecker.IsGrounded)
            {
                _dashTimer.Start();
            }
            else if (!performed && !_dashTimer.IsRunning)
            {
                _dashTimer.Stop();
            }
        }
        
        private void OnAttack()
        {
            if (!_attackCooldownTimer.IsRunning)
                _attackCooldownTimer.Start();
        }
        
        public void Attack()
        {
            var pos = transform.position + Vector3.forward;
            var hits = Physics.OverlapSphere(pos, _attackDamage);
            foreach (var hit in hits)
            {
                if (hit.CompareTag(Const.EnemyTag))
                {
                    hit.GetComponent<HealthComp>().TakeDamage(_attackDamage);
                }
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
            var adjustedDirection = Quaternion.AngleAxis(_mainCamTransform.eulerAngles.y, Vector3.up) * Movement;

            if (adjustedDirection.magnitude > ZeroF
                && _currentStateHash != AttackHash)
            {
                HandleRotation(adjustedDirection);
                HandleHorizontalMovement(adjustedDirection);
                // SmoothSpeed(adjustedDirection.magnitude);
            }
            else
            {
                // SmoothSpeed(ZeroF);

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
            var moveSpeed = IsRunning ? _runSpeed : _walkSpeed;

            // Move the player
            Vector3 velocity = adjustedDirection * (moveSpeed * _dashVelocity * Time.fixedDeltaTime);
            _rb.velocity = new Vector3(velocity.x, _rb.velocity.y, velocity.z);
        }
        
        // private void SmoothSpeed(float value)
        // {
        //     _currentSpeed = Mathf.SmoothDamp(_currentSpeed, value, ref _velocity, _smoothTime);
        // }
        
        private void UpdateAnimator()
        {
            // _animator.SetFloat(Speed, _currentSpeed);
            _animator.SetFloat(Speed, _rb.velocity.magnitude);
        }
    }
}