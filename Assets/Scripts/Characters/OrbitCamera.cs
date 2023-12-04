using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    private const float MIN_CENTERING_DISTANCE = 0.01f; // ignore centering if distance < this
    private const float INPUT_NOISE = 0.001f;

    [Header("Camera Focus")]
    [SerializeField]                    Transform focusTarget = default;
    [SerializeField, Range(1f, 20f)]    float focusDistance = 5f;
    [SerializeField, Min(0f)]           float focusRadius = 1f;
    [SerializeField, Range(0f, 1f)]     float focusReponsiveness = 0.5f; // 0.5f = mean halve distance/sec

    [Header("Camera Orbit")]
    [SerializeField, Range(1f, 360f)]   float rotationSpeed = 90f; // 90 = 90 degree/sec
    [SerializeField, Range(-89f, 89f)]  float minVerticalAngle = -30f;
    [SerializeField, Range(-89f, 89f)]  float maxVerticalAngle = 60f;

    private Vector3 _focusPoint = new Vector3();
    private Vector2 _orbitAngles = new Vector2(45f, 0f);

    private void Awake()
    {
        _focusPoint = focusTarget.position;
        transform.localRotation = Quaternion.Euler(_orbitAngles);
    }

    private void OnValidate()
    {
        if (maxVerticalAngle < minVerticalAngle)
            maxVerticalAngle = minVerticalAngle;
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();
        ManualRotation();

        Quaternion lookRotation;
        if (ManualRotation())
        {
            ConstrainAngles();
            lookRotation = Quaternion.Euler(_orbitAngles);
        }
        else
        {
            lookRotation = transform.localRotation;
        }

        Vector3 lookDirection = lookRotation * Vector3.forward;
        Vector3 lookPosition = _focusPoint - lookDirection * focusDistance;

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    // https://catlikecoding.com/unity/tutorials/movement/orbit-camera/#1.4
    private void UpdateFocusPoint()
    {
        Vector3 targetPoint = focusTarget.position;
        float distance = Vector3.Distance(targetPoint, _focusPoint);

        float t = 1f;
        if (distance > MIN_CENTERING_DISTANCE)
            t = Mathf.Pow(1 - focusReponsiveness, Time.unscaledDeltaTime);
        if (distance > focusRadius)
            t = Mathf.Min(t, focusRadius / distance);

        _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
    }

    private bool ManualRotation()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Camera Vertical"),
            Input.GetAxis("Camera Horizontal"));

        if (Mathf.Abs(input.x) > INPUT_NOISE || Mathf.Abs(input.y) > INPUT_NOISE)
        {
            _orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
            return true;
        }
        else
        {
            return false;
        }            
    }

    private void ConstrainAngles()
    {
        _orbitAngles.x = Mathf.Clamp(_orbitAngles.x, minVerticalAngle, maxVerticalAngle);

        if (_orbitAngles.y < 0)
            _orbitAngles.y += 360f;
        else if (_orbitAngles.y > 360)
            _orbitAngles.y -= 360f;
    }
}
