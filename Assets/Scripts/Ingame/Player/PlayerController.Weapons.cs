using KBCore.Refs;
using UnityEngine;

namespace StartledSeal
{
    public partial class PlayerController
    {
        public PlayerWeaponController PlayerWeaponControllerComp => _playerWeaponController;
        
        [SerializeField, Child] private PlayerWeaponController _playerWeaponController;
        
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
            if (!_playerWeaponController.IsAttacking() && _playerWeaponController.CanAttack(itemIndex))
                _playerWeaponController.Attack(itemIndex);
        }
        
        private void OnToggleShield(bool isUsingShield)
        {
            IsRotationLocked = IsUsingShield = isUsingShield;
            _playerWeaponController.ToggleShield(isUsingShield);
        }
    }
}