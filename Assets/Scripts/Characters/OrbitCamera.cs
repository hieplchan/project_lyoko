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
    [SerializeField, Min(0f)]           float focusCentering = 0.5f; // 0.5f mean halve distance/sec
    [SerializeField, Min(0f)]           float minCenteringDistance = 0.01f; // ignore if centering distance 

    private Vector3 _focusPoint;

    private void Awake()
    {
        _focusPoint = focus.position;
    }

    private void LateUpdate()
    {
        UpdateFocusPoint();

        transform.localPosition = _focusPoint - transform.forward * distance;
    }

    // https://catlikecoding.com/unity/tutorials/movement/orbit-camera/#1.4
    private void UpdateFocusPoint()
    {
        Vector3 targetPoint = focus.position;
        float distance = Vector3.Distance(targetPoint, _focusPoint);

        float t = 1f;
        if (distance > minCenteringDistance)
            t = Mathf.Pow(1 - focusCentering, Time.unscaledDeltaTime);
        if (distance > focusRadius)
            t = Mathf.Min(t, focusRadius / distance);

        _focusPoint = Vector3.Lerp(targetPoint, _focusPoint, t);
    }
}
