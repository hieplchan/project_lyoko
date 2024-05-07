using System;
using System.Threading.Tasks;
using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace StartledSeal
{
    public class BreakableObject: MonoBehaviour, IDamageable
    {
        [SerializeField] private ParticleSystem _vfx;
        [SerializeField] private GameObject _model;
        [SerializeField] private Collider _collider;
        
        [Button]
        public async UniTask TakeDamage(int damageAmount)
        {
            _vfx!.Play();
            _model.gameObject.SetActive(false);
            _collider.enabled = false;

            await UniTask.WaitUntil(() => _vfx.isStopped);
            Destroy(gameObject);
        }
    }
}