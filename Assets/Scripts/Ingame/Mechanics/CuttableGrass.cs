using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartledSeal
{
    public sealed class CuttableGrass : MonoBehaviour, IDamageable
    {
        [SerializeField] private ParticleSystem _vfx;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private Collider _collider;

        public async UniTask TakeDamage(int damageAmount)
        {
            if (_vfx != null && !_vfx.isPlaying)
                _vfx!.Play();
            _meshRenderer.enabled = false;
            if (_collider != null)
                _collider.enabled = false;

            if (_vfx != null)
                await UniTask.WaitUntil(() => _vfx.isStopped);
            Destroy(gameObject);
        }
    }
}