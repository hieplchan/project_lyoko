using Cinemachine;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public class CameraManager : ValidatedMonoBehaviour
    {
        [Header("References")] 
        [SerializeField, Anywhere] private InputReader _input;
        [SerializeField, Anywhere] private CinemachineFreeLook _freeLookCam;

        [Header("Settings")] 
        [SerializeField, Range(0.0f, 3f)] private float _speedMultiplier = 1f;
        [SerializeField, Range(1f, 1000f)] private float _gamepadMultiplier = 100f;

        private bool _isRightMouseBtnPress;
        private bool _isDeviceMouse;
        private bool _cameraMovementLock;

        private void OnEnable()
        {
            _input.Look += OnLook;
            _input.EnableMouseControlCamera += OnEnableMouseControlCamera;
            _input.DisableMouseControlCamera += OnDisableMouseControlCamera;
        }

        private void OnDisable()
        {
            _input.Look -= OnLook;
            _input.EnableMouseControlCamera -= OnEnableMouseControlCamera;
            _input.DisableMouseControlCamera -= OnDisableMouseControlCamera;
        }
        
        private void OnLook(Vector2 cameraMovement, bool isDeviceMouse)
        {
            if (_cameraMovementLock) return;
            if (isDeviceMouse && !_isRightMouseBtnPress) return;

            // if the device is mouse use fixedDeltaTime, otherwise use deltaTime
            float deviceMultiplier = isDeviceMouse ? Time.fixedDeltaTime : Time.deltaTime * _gamepadMultiplier;
            
            // Set camera axis value
            _freeLookCam.m_XAxis.m_InputAxisValue = cameraMovement.x * deviceMultiplier * _speedMultiplier;
            _freeLookCam.m_YAxis.m_InputAxisValue = cameraMovement.y * deviceMultiplier * _speedMultiplier;
        }
        
        private async void OnEnableMouseControlCamera()
        {   
            _isRightMouseBtnPress = true;
            
            // lock cursor to centor of screen and hide it
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // disable mouse for one frame to prevent weird hiccups
            _cameraMovementLock = true;
            await UniTask.WaitForEndOfFrame(this);
            _cameraMovementLock = false;
        }
        
        private void OnDisableMouseControlCamera()
        {
            _isRightMouseBtnPress = false;
         
            // unlock cursor to and show it
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            // reset camera axis to prevent jumping when re-enabling mouse control
            _freeLookCam.m_XAxis.m_InputAxisValue = 0f;
            _freeLookCam.m_YAxis.m_InputAxisValue = 0f;
        }
    }
}