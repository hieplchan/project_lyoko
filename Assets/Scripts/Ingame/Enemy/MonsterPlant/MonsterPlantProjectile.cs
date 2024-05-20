using System;
using Cysharp.Threading.Tasks;
using KBCore.Refs;
using StartledSeal.Common;
using Unity.VisualScripting;
using UnityEngine;

namespace StartledSeal
{
    public class MonsterPlantProjectile : ValidatedMonoBehaviour, IDamageable
    {
        [SerializeField, Self] private Rigidbody _rb;
        [SerializeField] private int damageAmount = 20;
        [SerializeField] private float lifeTimeSec = 10;
        [SerializeField] private float _projectileReverseVelocity = 700f;

        private float _awakeTimePoint;

        private void Awake()
        {
            _awakeTimePoint = Time.time;
        }

        public UniTask TakeDamage(int damageAmount, Transform impactObject)
        {
            Vector3 impactVector = impactObject.position - transform.position;
            impactVector.y = 0;
            _rb.AddRelativeForce(impactVector * _projectileReverseVelocity);
            return UniTask.CompletedTask;
        }

        private void OnTriggerEnter(Collider other)
        {
            MLog.Debug("MonsterPlantProjectile", $"OnTriggerEnter {other.name}");
            var damageableObj = other.gameObject.GetComponent<IDamageable>();

            if (damageableObj != null)
            {
                MLog.Debug("MonsterPlantProjectile", $"TakeDamage {damageableObj}");

                if (other.CompareTag(Const.PlayerTag) || other.CompareTag(Const.EnemyTag))
                {
                    damageableObj.TakeDamage(damageAmount, transform);
                    Destroy(gameObject);
                }
            }
        }

        private void Update()
        {
            if (Time.time > _awakeTimePoint + lifeTimeSec)
                Destroy(gameObject);
        }
    }
}