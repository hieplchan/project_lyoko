using System;
using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [SerializeField] private float _attackDistance = 1f;
        [SerializeField] private float _attackAngle = 60f;
        [SerializeField] private int _attackDamage = 10;
        
        public void Attack()
        {
            var pos = transform.position + Vector3.forward;
            var hits = Physics.OverlapSphere(pos, _attackDistance);
            foreach (var hit in hits)
            {
                var damageableObj = hit.gameObject.GetComponent<IDamageable>();
                // Check if hit in attack cone
                if (damageableObj != null)
                {
                    var directionToPlayer = hit.gameObject.transform.position - transform.position;
                    var angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                    
                    // MLog.Debug("PlayerWeaponController", $"angleToPlayer {angleToPlayer}");
                    
                    if (Mathf.Abs(angleToPlayer) < _attackAngle / 2)
                        damageableObj.TakeDamage(_attackDamage);
                }

                // if (hit.CompareTag(Const.EnemyTag))
                // {
                //     hit.GetComponent<Enemy>().GetHit(_attackDamage);
                // }
            }

            // foreach (var enemy in _playerFlashLightController.EnemiesInRangeList)
            // {
            //     enemy.GetHit(_attackDamage);
            // }
        }

        private void OnDrawGizmos()
        {
            // Draw attack cone
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _attackDistance);
            Vector3 forwardConeDirection = Quaternion.Euler(0, _attackAngle / 2, 0) * transform.forward * _attackDistance;
            Vector3 backwardConeDirection = Quaternion.Euler(0, -_attackAngle / 2, 0) * transform.forward * _attackDistance;
            Gizmos.DrawLine(transform.position, transform.position + forwardConeDirection);
            Gizmos.DrawLine(transform.position, transform.position + backwardConeDirection);
        }
    }
}