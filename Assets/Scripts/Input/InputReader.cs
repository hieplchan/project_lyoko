using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace StartledSeal
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "StartledSeal/Player/InputReader")]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions
    {
        public event UnityAction<Vector2> Move = delegate { };
        public event UnityAction<Vector2, bool> Look = delegate { };
        public event UnityAction EnableMouseControlCamera = delegate { };
        public event UnityAction DisableMouseControlCamera = delegate { };
        public event UnityAction<bool> Jump = delegate { };
        public event UnityAction<bool> Dash = delegate { };
        public event UnityAction<bool> Run = delegate { };
        public event UnityAction Attack = delegate { };
        public event UnityAction Equip = delegate { };
        
        public event UnityAction<bool> Item1 = delegate { };
        public event UnityAction<bool> Item2 = delegate { };
        public event UnityAction<bool> Item3 = delegate { };
        public event UnityAction<bool> Shield = delegate { }; 

        
        public Vector3 Direction => _inputActions.Player.Move.ReadValue<Vector2>();
        private PlayerInputActions _inputActions;

        private void OnEnable()
        {
            if (_inputActions == null)
            {
                _inputActions = new PlayerInputActions();
                _inputActions.Player.SetCallbacks(this);
            }
        }

        public void EnableplayerActions()
        {
            _inputActions.Enable();
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            Move.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            Look.Invoke(context.ReadValue<Vector2>(), IsDeviceMouse(context));
        }

        private bool IsDeviceMouse(InputAction.CallbackContext context) => context.control.device.name == "Mouse";

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Attack.Invoke();
            }
        }

        public void OnMouseControlCamera(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    EnableMouseControlCamera.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    DisableMouseControlCamera.Invoke();
                    break;
            }
        }

        public void OnRun(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Run.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Run.Invoke(false);
                    break;
            }
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Jump.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Jump.Invoke(false);
                    break;
            }
        }

        public void OnEquip(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Equip.Invoke();
                    break;
                // case InputActionPhase.Canceled:
                //     Equip.Invoke();
                //     break;
            };
        }

        public void OnItem1(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Item1.Invoke(true);
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                Item1.Invoke(false);
            }
        }

        public void OnItem2(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Item2.Invoke(true);
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                Item1.Invoke(false);
            }
        }

        public void OnItem3(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
            {
                Item3.Invoke(true);
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                Item1.Invoke(false);
            }
        }

        public void OnShield(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Shield.Invoke(true);
                    break;
                case InputActionPhase.Canceled:
                    Shield.Invoke(false);
                    break;
            };
        }
    }
}
