using KBCore.Refs;
using UnityEngine;

namespace StartledSeal
{
    public class WaterChecker : ValidatedMonoBehaviour
    {
        [SerializeField, Self] private CapsuleCollider _capsuleCollider;
        [SerializeField] private LayerMask _waterLayers;
        [SerializeField] private float _inWaterHeightThreshold = .7f; // distance start from top head
        
        public bool IsInWater { get; private set; }
        
        private void Update()
        {
            IsInWater = Physics.Raycast(transform.position + _capsuleCollider.height * Vector3.up,
                Vector3.down, _inWaterHeightThreshold, _waterLayers);
        }
        
        private void OnDrawGizmosSelected() 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + _capsuleCollider.height * Vector3.up, Vector3.down * _inWaterHeightThreshold);
        }
    }
}