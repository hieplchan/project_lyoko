using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    private const float MIN_CENTERING_DISTANCE = 0.01f; // ignore centering if distance < this
    private const float INPUT_NOISE = 0.001f;
    private const float MIN_MOVEMENT_SQR_MAGNITUDE = 0.0001f;

    [Header("Camera Focus")]
    [SerializeField]                    Transform focusTarget = default;
    [SerializeField, Range(1f, 20f)]    float focusDistance = 5f;
    [SerializeField, Min(0f)]           float focusRadius = 1f;
    [SerializeField, Range(0f, 1f)]     float focusReponsiveness = 0.5f; // 0.5f = mean halve distance/sec

    [Header("Camera Orbit")]
    [SerializeField, Range(1f, 360f)]   float rotationSpeed = 90f; // 90 = 90 degree/sec
    [SerializeField, Range(-89f, 89f)]  float minVerticalAngle = -30f;
    [SerializeField, Range(-89f, 89f)]  float maxVerticalAngle = 60f;
    [SerializeField, Min(0f)]           float horizontalAlignDelay = 5f; // honizontal = behind character

    private Vector3 _focusPoint, _prevFocusPoint;
    private Vector2 _orbitAngles = new Vector2(45f, 0f);
    private float _lastManualRotationTime;

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
        if (ManualRotation() || AutomaticRotation())
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
        _prevFocusPoint = _focusPoint;

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
            _lastManualRotationTime = Time.unscaledTime;
            return true;
        }
        else
        {
            return false;
        }            
    }

    private bool AutomaticRotation()
    {
        if (Time.unscaledTime - _lastManualRotationTime < horizontalAlignDelay) 
            return false;

        // Top down XZ plane
        Vector2 movement = new Vector2(
            _focusPoint.x - _prevFocusPoint.x,
            _focusPoint.z - _prevFocusPoint.z);

        float movementDeltaSqr = movement.sqrMagnitude;
        if (movementDeltaSqr < MIN_MOVEMENT_SQR_MAGNITUDE) return false;

        float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
        _orbitAngles.y = headingAngle;

        return true;
    }

    // https://catlikecoding.com/unity/tutorials/movement/orbit-camera/#2.5
    static float GetAngle(Vector2 direction)
    {
        float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
        return direction.x < 0f ? 360f - angle : angle;
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
