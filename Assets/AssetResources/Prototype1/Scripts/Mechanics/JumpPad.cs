using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class JumpPad : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _speed = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigidBody = other.attachedRigidbody;
        if (rigidBody) Accelerate(rigidBody);
    }

    private void Accelerate(Rigidbody body)
    {
        Vector3 velocity = transform.InverseTransformDirection(body.velocity);
        if (velocity.y >= _speed) return;

        velocity.y = _speed;

        body.velocity = transform.TransformDirection(velocity);
        if (body.TryGetComponent<Character>(out var character))
        {
            character.AnimController.SetAnim(CharacterAnimKey.Jump);
        }
    }
}
