using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartledSeal
{
    public class Sword : BaseEquipment
    {
        [Header("Sword Settings")]
        [SerializeField] private float _normalAttackDistance = 1.8f;
        [SerializeField] private float _normalAttackAngle = 180f;
        [SerializeField] private int _normalAttackDamage = 10;

        [SerializeField] private float _chargedAttackDistance = 1.8f;
        [SerializeField] private float _chargedAttackAngle = 360f;
        [SerializeField] private int _chargedAttackDamage = 20;

        [SerializeField] private float _attackScaleUp = 2f;
        
        public override void NormalAttack()
        {
            base.NormalAttack();
            transform.localScale = _attackScaleUp * Vector3.one;
            ConeAttack(_normalAttackDistance, _normalAttackAngle, _normalAttackDamage);
        }
        
        public override void ChargedAttack()
        {
            base.ChargedAttack();
            ConeAttack(_chargedAttackDistance, _chargedAttackAngle, _chargedAttackDamage);
        }

        public override void StartCharging()
        {
            base.StartCharging();
            transform.localScale = _attackScaleUp * Vector3.one;
        }

        public override void StopUsing()
        {
            base.StopUsing();
            transform.localScale = Vector3.one;
        }

        private void ConeAttack(float attackDistance, float attackAngle, int attackDamage)
        {
            var _originTransform = _weaponController.gameObject.transform;
            
            var pos = _originTransform.position + Vector3.forward;
            var hits = Physics.OverlapSphere(pos, attackDistance);
            foreach (var hit in hits)
            {
                var damageableObj = hit.gameObject.GetComponent<IDamageable>();
                // Check if hit in attack cone
                if (damageableObj != null)
                {
                    var directionToPlayer = hit.gameObject.transform.position - _originTransform.position;
                    var angleToPlayer = Vector3.Angle(_originTransform.forward, directionToPlayer);
                    
                    // MLog.Debug("PlayerWeaponController", $"angleToPlayer {angleToPlayer}");
                    
                    if (Mathf.Abs(angleToPlayer) < attackAngle / 2)
                        damageableObj.TakeDamage(AttackType.Sword, attackDamage, _originTransform);
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!_isShowGizmos) return;
            
            var _originTransform = _weaponController.gameObject.transform;

            // Draw normal attack cone
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_originTransform.position, _normalAttackDistance);
            Vector3 forwardConeDirection = Quaternion.Euler(0, _normalAttackAngle / 2, 0) * _originTransform.forward * _normalAttackDistance;
            Vector3 backwardConeDirection = Quaternion.Euler(0, -_normalAttackAngle / 2, 0) * _originTransform.forward * _normalAttackDistance;
            Gizmos.DrawLine(_originTransform.position, _originTransform.position + forwardConeDirection);
            Gizmos.DrawLine(_originTransform.position, _originTransform.position + backwardConeDirection);
            
            // Draw charged attack cone
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_originTransform.position, _chargedAttackDistance);
            forwardConeDirection = Quaternion.Euler(0, _chargedAttackAngle / 2, 0) * _originTransform.forward * _chargedAttackDistance;
            backwardConeDirection = Quaternion.Euler(0, -_chargedAttackAngle / 2, 0) * _originTransform.forward * _chargedAttackDistance;
            Gizmos.DrawLine(_originTransform.position, _originTransform.position + forwardConeDirection);
            Gizmos.DrawLine(_originTransform.position, _originTransform.position + backwardConeDirection);
        }
    }
}