using System;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StartledSeal
{
    public class Shield : BaseEquipment, IDamageable
    {
        [SerializeField] private float _fallbackForce = 700f;
        
        public override bool IsUsable() => true;

        public override async UniTask Use(Animator animatorComp)
        {
            animatorComp.CrossFade(_animHash, 0.001f);
            animatorComp.SetLayerWeight(1, 1);
        }

        public async UniTask DisableShield(Animator animatorComp)
        {
            animatorComp.SetLayerWeight(1, 0);
        }

        public UniTask TakeDamage(AttackType attackType, int damageAmount, Transform impactObject)
        {
            if (_vfx != null)
                _vfx.Play();
            
            // ApplyFallbackForce();
            
            if (attackType == AttackType.Projectile)
            {
                var projectile = impactObject.gameObject.GetComponent<IDamageable>();
                projectile.TakeDamage(AttackType.Projectile, 20, 
                    _weaponController.gameObject.transform);
            }

            return UniTask.CompletedTask;
        }
        
        [Button]
        private void ApplyFallbackForce()
        {
            Vector3 impactVector = -_weaponController.PlayerControllerComp.transform.forward;
            impactVector.y = 0;
            _weaponController.PlayerControllerComp.RigidBody
                .AddForce(impactVector * _fallbackForce);
        }
    }
}