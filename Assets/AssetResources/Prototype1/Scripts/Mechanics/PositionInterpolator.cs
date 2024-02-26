using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PositionInterpolator : MonoBehaviour
{
    [SerializeField] private Rigidbody _body = default;
    [SerializeField] private Vector3 _from = default, _to = default;
    [SerializeField] Transform _relativeTo = default;

    public void Interpolate(float value)
    {
        Vector3 p;
        if (_relativeTo)
            p = Vector3.LerpUnclamped(_relativeTo.TransformPoint(_from), _relativeTo.TransformPoint(_to), value);
        else
            p = Vector3.LerpUnclamped(_from, _to, value);
        _body.MovePosition(p);
    }

#if UNITY_EDITOR
    [Button] void CaptureFromPosition() => _from = _relativeTo.InverseTransformPoint(_body.gameObject.transform.position);
    [Button] void CaptureToPosition() => _to = _relativeTo.InverseTransformPoint(_body.gameObject.transform.position);
    [Button] void TestFromPosition() => _body.gameObject.transform.position = _relativeTo.TransformPoint(_from);
    [Button] void TestToPosition() => _body.gameObject.transform.position = _relativeTo.TransformPoint(_to);

#endif
}
