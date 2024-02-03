using System;
using StartledSeal.Utils;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerDetector : MonoBehaviour
    {
        [SerializeField] private float _detectionAngle = 60f; // Cone in front of enemy
        [SerializeField] private float _detectionRadius = 10f; // Large circle around enemy
        [SerializeField] private float _innerDetectionRadius = 3f; // Small circle around enemy
        [SerializeField] private float _detectionCoolDown = 1f; // Time bettween detection
        [SerializeField] float _attackRange = 2f; // Distance from enemy to player to attack
        
        public Transform Player { get; private set; }
        public Health PlayerHealth { get; private set; }
        
        private CooldownTimer _detectionTimer;

        private IDetectionStrategy _detectionStrategy;

        private void Awake()
        {
            Player = GameObject.FindGameObjectWithTag(Const.PlayerTag).transform;
            PlayerHealth = Player.GetComponent<Health>();
        }
        
        private void Start()
        {
            _detectionTimer = new CooldownTimer(_detectionCoolDown);
            _detectionStrategy = new ConeDetectionStrategy(_detectionAngle, _detectionRadius, _innerDetectionRadius);
        }

        private void Update() => _detectionTimer.Tick(Time.deltaTime);

        public bool CanDetectPlayer() => _detectionTimer.IsRunning || _detectionStrategy.Execute(Player, transform, _detectionTimer);

        public bool CanAttackPlayer()
        {
            var directionToPlayer = Player.position - transform.position;
            return directionToPlayer.magnitude <= _attackRange;        
        }
        
        void OnDrawGizmos() {
            Gizmos.color = Color.red;

            // Draw a spheres for the radii
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
            Gizmos.DrawWireSphere(transform.position, _innerDetectionRadius);

            // Calculate our cone directions
            Vector3 forwardConeDirection = Quaternion.Euler(0, _detectionAngle / 2, 0) * transform.forward * _detectionRadius;
            Vector3 backwardConeDirection = Quaternion.Euler(0, -_detectionAngle / 2, 0) * transform.forward * _detectionRadius;

            // Draw lines to represent the cone
            Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
            Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _attackRange);
        }
    }
}