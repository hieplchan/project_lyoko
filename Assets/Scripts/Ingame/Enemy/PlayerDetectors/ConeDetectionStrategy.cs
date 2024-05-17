using StartledSeal.Utils;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace StartledSeal
{
    public class ConeDetectionStrategy : IDetectionStrategy
    {
        private readonly float _detectionAngle;
        private readonly float _detectionRadius;
        private readonly float _innerDetectionRadius;
        
        public ConeDetectionStrategy(float detectionAngle, float detectionRadius, float innerDetectionRadius)
        {
            _detectionAngle = detectionAngle;
            _detectionRadius = detectionRadius;
            _innerDetectionRadius = innerDetectionRadius;
        }

        public bool Execute(Transform player, Transform detector, CooldownTimer timer)
        {
            if (timer.IsRunning) return false;

            var directionToPlayer = player.position - detector.position;
            var angleToPlayer = Vector3.Angle(directionToPlayer, detector.forward);
            
            // If player not within detection angle + outer radius (cone from of enemy)
            // Or if player in inner radius, return false
            if ((!(angleToPlayer < _detectionAngle / 2f) || !(directionToPlayer.magnitude < _detectionRadius))
                && !(directionToPlayer.magnitude < _innerDetectionRadius))
                return false;
            
            timer.Start();
            return true;
        }
    }
}