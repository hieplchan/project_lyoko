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

        public override void Use(bool active) { }

        public override void Update() { }

        public void EnableShield()
        {
            CurrentState = EquipmentState.NormalAttackState;
            _player.AnimatorComp.SetLayerWeight(Const.UpperBodyAnimLayer, 1);
            _player.AnimatorComp.Play(_animNormalAttackHash, 1, 0f);
        }

        public void DisableShield()
        {
            CurrentState = EquipmentState.NotBeingUsed;
            _player.AnimatorComp.SetLayerWeight(Const.UpperBodyAnimLayer, 0);
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