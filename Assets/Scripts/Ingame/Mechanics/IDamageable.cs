using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartledSeal
{
    public enum AttackType
    {
        None = 0,
        Sword = 1,
        Gun = 2,
        Projectile = 3,
    }
    
    public interface IDamageable
    {
        public UniTask TakeDamage(AttackType attackType, int damageAmount, Transform impactObject);
    }
}