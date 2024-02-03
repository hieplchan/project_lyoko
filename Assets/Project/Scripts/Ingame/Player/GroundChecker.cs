using System;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.Serialization;

namespace StartledSeal
{
    public class GroundChecker : MonoBehaviour
    {
        private const float HeightOffset = 0.1f;

        [SerializeField, Self] private CapsuleCollider _capsuleCollider;
        [SerializeField] private LayerMask _groundLayers;

        public bool IsGrounded { get; private set; }

        private float _groundDistance;
        
        private void Awake()
        {
            _groundDistance = _capsuleCollider.height / 2;
        }

        private void Update()
        {
            IsGrounded = Physics.SphereCast(
                transform.position + new Vector3(0f, _groundDistance + HeightOffset, 0f), 
                _groundDistance, Vector3.down, 
                out _, HeightOffset * 2, _groundLayers);
        }

        // private void OnDrawGizmosSelected() 
        // {
        //     // Start point
        //     Gizmos.color = Color.yellow;
        //     Gizmos.DrawWireSphere(transform.position + new Vector3(0f, _groundDistance + HeightOffset, 0f), _groundDistance);
        //     
        //     // End point
        //     Gizmos.color = Color.red;
        //     Gizmos.DrawWireSphere(transform.position + new Vector3(0f, _groundDistance + HeightOffset - HeightOffset *2, 0f), _groundDistance);
        // }
    }
}