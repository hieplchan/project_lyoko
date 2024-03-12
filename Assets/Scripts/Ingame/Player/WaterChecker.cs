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
        public float SubmergedLevel { get; private set; }
        
        private void Update()
        {
            if (Physics.Raycast(transform.position + _capsuleCollider.height * Vector3.up,
                    Vector3.down, out var hit, _capsuleCollider.height, _waterLayers))
            {
                IsInWater = hit.distance < _inWaterHeightThreshold;
                SubmergedLevel = 1 - hit.distance / _capsuleCollider.height;
            }
            else
            {
                IsInWater = false;
                SubmergedLevel = 0;
            }
        }
        
        private void OnDrawGizmosSelected() 
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position + _capsuleCollider.height * Vector3.up, Vector3.down * _inWaterHeightThreshold);
        }
    }
}