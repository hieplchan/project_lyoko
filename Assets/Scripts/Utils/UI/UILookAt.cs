using System;
using UnityEngine;

namespace StartledSeal.Utils.UI
{
    public class UILookAt : MonoBehaviour
    {
        private Transform _mainCamera;
        
        private void Awake()
        {
            _mainCamera = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (_mainCamera != null)
                transform.LookAt(transform.position + _mainCamera.rotation * Vector3.forward, _mainCamera.rotation * Vector3.up);
        }
    }
}