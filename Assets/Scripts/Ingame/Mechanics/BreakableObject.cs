using System;
using System.Threading.Tasks;
using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace StartledSeal
{
    public sealed class BreakableObject: MonoBehaviour, IDamageable
    {
        [SerializeField] private ParticleSystem _vfx;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;
        
        [Button]
        public async UniTask TakeDamage(int damageAmount)
        {
            if (_vfx != null && !_vfx.isPlaying)
                _vfx!.Play();
            _meshRenderer!.enabled = false;
            if (_collider != null)
                _collider.enabled = false;

            if (_vfx != null)
                await UniTask.WaitUntil(() => _vfx.isStopped);
            Destroy(gameObject);
        }
    }
}