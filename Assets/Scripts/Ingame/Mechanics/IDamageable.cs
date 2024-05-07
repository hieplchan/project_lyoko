using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace StartledSeal
{
    public interface IDamageable
    {
        public UniTask TakeDamage(int damageAmount);
    }
}