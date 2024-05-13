using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartledSeal
{
    public interface IDamageable
    {
        public UniTask TakeDamage(int damageAmount, Transform impactObject);
    }
}