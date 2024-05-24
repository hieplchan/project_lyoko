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
        
        private void OnItem1() => UseItem(0);
        private void OnItem2() => UseItem(1);
        private void OnItem3() => UseItem(2);
        
        private void UseItem(int itemIndex)
        {
            if (!PlayerWeaponControllerComp.IsAttacking() && PlayerWeaponControllerComp.CanAttack(itemIndex))
                PlayerWeaponControllerComp.Attack(itemIndex);
        }
        
        private void OnToggleShield(bool isUsingShield)
        {
            IsRotationLocked = IsUsingShield = isUsingShield;
            PlayerWeaponControllerComp.ToggleShield(isUsingShield);
        }
    }
}