using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;

namespace StartledSeal
{
    public abstract class BaseEquipment : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] protected PlayerWeaponController _weaponController;
        [SerializeField] ParticleSystem _vfx;
        [SerializeField] private string AnimState;

        protected int _animHash;

        private void Awake()
        {
            _animHash = Animator.StringToHash(AnimState);
        }

        public abstract bool IsUsable();

        public virtual UniTask Use(Animator _animatorComp)
        {
            _animatorComp.CrossFade(_animHash, 0.001f);
            if (_vfx != null) _vfx.Play();
            
            return UniTask.CompletedTask;
        }
    }
}