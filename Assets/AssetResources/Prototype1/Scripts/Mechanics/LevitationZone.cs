using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class LevitationZone : MonoBehaviour
{
    [SerializeField, Min(0f)] 
    private float _acceleration = 10f, _speed = 10f;

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rigidBody = other.attachedRigidbody;
        if (rigidBody) Accelerate(rigidBody);
    }

    private void Accelerate(Rigidbody body)
    {
        Vector3 velocity = transform.InverseTransformDirection(body.velocity);
        if (velocity.y >= _speed) return;

        velocity.y = Mathf.MoveTowards(velocity.y, _speed, _acceleration * Time.deltaTime);

        body.velocity = transform.TransformDirection(velocity);
        if (body.TryGetComponent<Character>(out var character))
        {
        }
    }
}
