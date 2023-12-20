using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public sealed class DetectionZone : MonoBehaviour
{
    [SerializeField]
    UnityEvent _firstObjectEntered = default, _lastObjectExited = default;

    private List<Collider> _colliders = new List<Collider>();

    private void Awake()
    {
        // Disable unneeded Fixed update check when no object in zone
        enabled = false;
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        // Avoid Editor hot reload
        if (enabled && gameObject.activeInHierarchy) return;
#endif

        if (_colliders.Count > 0)
        {
            _colliders.Clear();
            _lastObjectExited.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_colliders.Count == 0)
        {
            _firstObjectEntered.Invoke();
            enabled = true;
        }

        _colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_colliders.Remove(other) && _colliders.Count == 0)
        {
            _lastObjectExited.Invoke();
            enabled = false;
        }
    }

    private void FixedUpdate()
    {
        // Just in case object being disable when in zone
        for (int i = 0; i < _colliders.Count; i++)
        {
            Collider collider = _colliders[i];
            if (!collider || !collider.gameObject.activeInHierarchy)
            {
                _colliders.RemoveAt(i--);
                if (_colliders.Count == 0)
                {
                    _lastObjectExited.Invoke();
                    enabled = false;
                }
            }
        }
    }
}
