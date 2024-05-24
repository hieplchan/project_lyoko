using System;
using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StartledSeal
{
    public class Shield : BaseEquipment, IDamageable
    {
        [SerializeField] private float _fallbackForce = 700f;
        [SerializeField] private AnimationSequencerController _getHitAnimSeq;

        public override bool IsUsable() => true;

        public override async UniTask NormalAttack(Animator animatorComp)
        {
            animatorComp.CrossFade(_animNormalAttackHash, 0.001f);
            animatorComp.SetLayerWeight(1, 1);
        }

        public async UniTask DisableShield(Animator animatorComp)
        {
            animatorComp.SetLayerWeight(1, 0);
        }

        public UniTask TakeDamage(AttackType attackType, int damageAmount, Transform impactObject)
        {
            if (_getHitAnimSeq != null && !_getHitAnimSeq.IsPlaying)
                _getHitAnimSeq.Play();
            
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
            _weaponController.PlayerControllerComp.RigidBodyComp
                .AddForce(impactVector * _fallbackForce);
        }
    }
}