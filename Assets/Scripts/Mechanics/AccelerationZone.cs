using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationZone : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _speed = 10f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rigidBody = other.attachedRigidbody;
        if (rigidBody) Accelerate(rigidBody);
    }

    private void Accelerate(Rigidbody body)
    {
        Vector3 velocity = body.velocity;
        if (velocity.y >= _speed) return;

        velocity.y = _speed;
        body.velocity = velocity;

        if (body.TryGetComponent<Character>(out var character))
        {
            character.AnimController.SetAnim(CharacterAnimKey.Jump);
        }
    }
}
