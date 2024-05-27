using System.Collections.Generic;
using StartledSeal.Utils;
using UnityEngine;

namespace StartledSeal
{
    public partial class PlayerController
    {
        // Timers
        private List<Timer> _timers;
        
        private CooldownTimer _jumpTimer; // how long user hold jump btn
        private CooldownTimer _jumpCooldownTimer; // wait to next jump

        private CooldownTimer _dashTimer;
        private CooldownTimer _dashCooldownTimer;

        // State Machine
        private StateMachine _stateMachine;
        private int _currentStateHash;
        
        private void SetupTimers()
        {
            _jumpTimer = new CooldownTimer(_jumpDuration);

            _dashTimer = new CooldownTimer(_dashDuration);
            _dashCooldownTimer = new CooldownTimer(_dashCoolDown);
            
            _timers = new List<Timer>(3) { _jumpTimer, _dashTimer, _dashCooldownTimer };

            _dashTimer.OnTimerStart += () => _dashVelocity = _dashForce;
            _dashTimer.OnTimerStop += () =>
            {
                _dashCooldownTimer.Start();
                _dashVelocity = 1f;
            };
        }
        
        private void HandleTimers()
        {
            foreach (var timer in _timers)
                timer.Tick(Time.deltaTime);
        }
        
        void At(IState from, IState to, IPredicate condition) => _stateMachine.AddTransition(from, to, condition);
        void Any(IState to, IPredicate condition) => _stateMachine.AddAnyTransition(to, condition);
        
        private void SetupStateMachine()
        {
            _stateMachine = new StateMachine();
            
            // declare state
            var locomotionState = new LocomotionState(this, _runStaminaConsumptionPerSec);
            var jumpState = new JumpState(this);
            var dashState = new DashState(this);
            var attackState = new AttackState(this);
            var deadState = new DeadState(this);
            // var flyState = new FlyState(this, _animator, _flyDrag, _rb.drag);
            var swimState = new SwimState(this);
            
            // define transition
            At(locomotionState, jumpState, new FuncPredicate(() => _jumpTimer.IsRunning));
            At(locomotionState, dashState, new FuncPredicate(() => _dashTimer.IsRunning));
            
            At(locomotionState, attackState, new FuncPredicate(() => PlayerWeaponControllerComp.IsAttacking));
            At(attackState, locomotionState, new FuncPredicate(() => !PlayerWeaponControllerComp.IsAttacking));
            
            At(dashState, jumpState, new FuncPredicate(() => !_dashTimer.IsRunning && _jumpTimer.IsRunning));
            At(jumpState, dashState, new FuncPredicate(() => _dashTimer.IsRunning && !_jumpTimer.IsRunning));
            
            // At(dashState, attackState, new FuncPredicate(() => !_dashTimer.IsRunning && _playerWeaponController.IsAttacking()));
            // At(attackState, dashState, new FuncPredicate(() => _dashTimer.IsRunning && !_playerWeaponController.IsAttacking()));
            
            At(locomotionState, deadState, new FuncPredicate(() => PlayerHealthComp.IsDead()));
            
            // At(jumpState, flyState, new FuncPredicate(() => !_groundChecker.IsGrounded && IsFlying));
            // At(flyState, jumpState, new FuncPredicate(() => !_groundChecker.IsGrounded && !IsFlying));
            
            At(swimState, locomotionState, new FuncPredicate(ReturnToLocomotionState));
            
            Any(locomotionState, new FuncPredicate(ReturnToLocomotionState));
            Any(swimState, new FuncPredicate(() => WaterCheckerComp.IsInWater));

            // set init state
            _stateMachine.SetState(locomotionState);
        }

        private bool ReturnToLocomotionState()
        {
            return GroundCheckerComp.IsGrounded
                   && !_jumpTimer.IsRunning
                   && !_dashTimer.IsRunning
                   && !PlayerWeaponControllerComp.IsAttacking
                   && !PlayerHealthComp.IsDead()
                   && !WaterCheckerComp.IsInWater;
        }

        public void SetStateHash(int stateHash)
        {
            _currentStateHash = stateHash;
        }
    }
}