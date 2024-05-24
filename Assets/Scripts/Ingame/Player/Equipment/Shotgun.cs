using CartoonFX;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartledSeal
{
    public class Shotgun : BaseEquipment
    {
        [SerializeField] private float _attackDistance = 1.8f;
        [SerializeField] private float _attackAngle = 180f;
        [SerializeField] private int _attackDamage = 10;

        [SerializeField] private bool _isUseCameraShake;
        [SerializeField] private CFXR_Effect _cameraEffect;

        [SerializeField] private bool _isShowGizmos;

        public override bool IsUsable() => true;
        
        public override async UniTask NormalAttack(Animator _animatorComp)
        {
            base.NormalAttack(_animatorComp);

            if (_isUseCameraShake && _cameraEffect != null)
            {
                _cameraEffect.ResetState();
                _cameraEffect.Animate(0f);   
            }
            
            var _originTransform = _weaponController.gameObject.transform;
            
            var pos = _originTransform.position + Vector3.forward;
            var hits = Physics.OverlapSphere(pos, _attackDistance);
            foreach (var hit in hits)
            {
                var damageableObj = hit.gameObject.GetComponent<IDamageable>();
                // Check if hit in attack cone
                if (damageableObj != null)
                {
                    var directionToPlayer = hit.gameObject.transform.position - _originTransform.position;
                    var angleToPlayer = Vector3.Angle(_originTransform.forward, directionToPlayer);
                    
                    // MLog.Debug("PlayerWeaponController", $"angleToPlayer {angleToPlayer}");
                    
                    if (Mathf.Abs(angleToPlayer) < _attackAngle / 2)
                        damageableObj.TakeDamage(AttackType.Gun, _attackDamage, _originTransform);
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            if (!_isShowGizmos) return;
            
            var _originTransform = _weaponController.gameObject.transform;

            // Draw attack cone
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_originTransform.position, _attackDistance);
            Vector3 forwardConeDirection = Quaternion.Euler(0, _attackAngle / 2, 0) * _originTransform.forward * _attackDistance;
            Vector3 backwardConeDirection = Quaternion.Euler(0, -_attackAngle / 2, 0) * _originTransform.forward * _attackDistance;
            Gizmos.DrawLine(_originTransform.position, _originTransform.position + forwardConeDirection);
            Gizmos.DrawLine(_originTransform.position, _originTransform.position + backwardConeDirection);
        }
    }
}