using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField]                    Transform focus = default;
    [SerializeField, Range(1f, 20f)]    float distance = 5f;
    [SerializeField, Min(0f)]           float focusRadius = 1f;
    [SerializeField, Min(0f)]           float reponsiveness = 0.5f; // focusCentering 0.5f = mean halve distance/sec
    [SerializeField, Min(0f)]           float minCenteringDistance = 0.01f; // ignore if centering distance < this
    [SerializeField, Range(1f, 360f)]   float rotationSpeed = 90f; // 90 = 90 degree/sec


    private Vector3 _focusPoint = new Vector3();
    private Vector2 _orbitAngles = new Vector2(45f, 0f);

    private void Awake()
    {
        _focusPoint = focus.position;
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();
        ManualRotation();

        Quaternion lookRotation = Quaternion.Euler(_orbitAngles);
        Vector3 lookDirection = lookRotation * transform.forward;
        Vector3 lookPosition = _focusPoint - lookDirection * distance;

        transform.SetPositionAndRotation(lookPosition, lookRotation);
    }

    // https://catlikecoding.com/unity/tutorials/movement/orbit-camera/#1.4
    private void UpdateFocusPoint()
    {
        Vector3 targetPoint = focus.position;
        float distance = Vector3.Distance(targetPoint, _focusPoint);

        float t = 1f;
        if (distance > minCenteringDistance)
            t = Mathf.Pow(1 - reponsiveness, Time.unscaledDeltaTime);
        if (distance > focusRadius)
            t = Mathf.Min(t, focusRadius / distance);

        _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
    }

    private void ManualRotation()
    {
        Vector2 input = new Vector2(
            Input.GetAxis("Vertical Camera"),
            Input.GetAxis("Horizontal Camera"));

        const float e = 0.001f; // input noise
        if (Mathf.Abs(input.x) > e || Mathf.Abs(input.y) > e)
            _orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
    }
}
