using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;

namespace StartledSeal
{
    public partial class PlayerController
    {
        [field: SerializeField, Child] public PlayerWeaponController PlayerWeaponControllerComp { get; private set; }

        public bool IsUsingShield;
        
        public void EnableUsingItem()
        {
            // _input.Attack += OnAttack;
            _input.Item1 += OnItem1;
            _input.Item2 += OnItem2;
            _input.Item3 += OnItem3;
            _input.Shield += OnToggleShield;
        }
        
        public void DisableUsingItem()
        {
            // _input.Attack -= OnAttack;
            _input.Item1 -= OnItem1;
            _input.Item2 -= OnItem2;
            _input.Item3 -= OnItem3;
            _input.Shield -= OnToggleShield;
        }
        
        private void OnItem1(bool active) => UseItem(0, active);
        private void OnItem2(bool active) => UseItem(1, active);
        private void OnItem3(bool active) => UseItem(2, active);
        
        private void UseItem(int itemIndex, bool active)
        {
            if (active)
            {
                if (!PlayerWeaponControllerComp.IsAttacking() && PlayerWeaponControllerComp.CanAttack(itemIndex))
                    PlayerWeaponControllerComp.Attack(itemIndex);
            }
        }
        
        private void OnToggleShield(bool isUsingShield)
        {
            IsRotationLocked = IsUsingShield = isUsingShield;
            PlayerWeaponControllerComp.ToggleShield(isUsingShield);
        }
    }
}