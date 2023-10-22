// https://catlikecoding.com/unity/tutorials/movement/orbit-camera/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField] Transform focus = default;
    [SerializeField, Range(1f, 20f)] float distance = 5f;
    [SerializeField, Min(0f)] float focusRadius = 1f;

    private Vector3 _focusPoint = Vector3.zero;
    private Vector3 _targetPoint = Vector3.zero;
    private Vector3 _lookDirection = Vector3.zero;

    private void Awake()
    {
        _focusPoint = focus.position;
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();
        _lookDirection = transform.forward;
        transform.localPosition = _focusPoint - _lookDirection * distance;
    }

    private void UpdateFocusPoint()
    {
        _targetPoint = focus.position;

        if (focusRadius > 0f)
        {
            float distance = Vector3.Distance(_targetPoint, _focusPoint);
            if (distance > focusRadius)
            {
                _focusPoint = Vector3.Lerp(_targetPoint, _focusPoint, focusRadius/distance);
            }
        }
        else
        {
            _focusPoint = _targetPoint;
        }

    }
}
