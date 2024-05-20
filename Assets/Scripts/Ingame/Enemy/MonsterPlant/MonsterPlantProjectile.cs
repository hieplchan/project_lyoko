using System;
using Cysharp.Threading.Tasks;
using StartledSeal.Common;
using Unity.VisualScripting;
using UnityEngine;

namespace StartledSeal
{
    public class MonsterPlantProjectile : MonoBehaviour, IDamageable
    {
        [SerializeField] private int damageAmount = 20;
        [SerializeField] private float lifeTimeSec = 10;

        private float _awakeTimePoint;

        private void Awake()
        {
            _awakeTimePoint = Time.time;
        }

        public UniTask TakeDamage(int damageAmount, Transform impactObject)
        {
            return UniTask.CompletedTask;
        }

        private void OnTriggerEnter(Collider other)
        {
            MLog.Debug("MonsterPlantProjectile", $"OnTriggerEnter {other.name}");
            
            var damageableObj = other.gameObject.GetComponent<IDamageable>();

            if (damageableObj != null)
            {
                MLog.Debug("MonsterPlantProjectile", $"TakeDamage {damageableObj}");
                damageableObj.TakeDamage(damageAmount, transform);
            }
            
            Destroy(gameObject);
        }

        private void Update()
        {
            if (Time.time > _awakeTimePoint + lifeTimeSec)
                Destroy(gameObject);
        }
    }
}