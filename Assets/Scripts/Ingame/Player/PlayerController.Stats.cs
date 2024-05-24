using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;

namespace StartledSeal
{
    public partial class PlayerController
    {
        [field: SerializeField, Child] public PlayerStaminaComp PlayerStaminaComp { get; private set; }
        [field: SerializeField, Child] public HealthComp PlayerHealthComp { get; private set; }
        
        public UniTask TakeDamage(AttackType attackType, int damageAmount, Transform impactObject)
        {
            GetHitEvent?.Invoke();
            PlayerHealthComp.TakeDamage(damageAmount);
            return UniTask.CompletedTask;
        }
        
        private void OnRunOutOfStamina() => IsRunning = false;
    }
}