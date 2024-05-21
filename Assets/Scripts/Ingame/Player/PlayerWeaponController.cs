using System;
using System.Collections.Generic;
using KBCore.Refs;
using StartledSeal.Common;
using StartledSeal.Utils;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerWeaponController : ValidatedMonoBehaviour
    {
        [SerializeField] private Transform _equipmentSpawnPoint;
        [SerializeField, Parent] private PlayerController _playerController;
        
        [SerializeField] private Shield _shield;
        [SerializeField] private List<BaseEquipment> _equipmentList;

        private CooldownTimer _attackCooldownTimer;
        private BaseEquipment _currentEquipment;

        private void Awake()
        {
            _currentEquipment = _equipmentList[0];
            _attackCooldownTimer = new CooldownTimer(_currentEquipment.AttackCoolDown);
            _attackCooldownTimer.Reset(_currentEquipment.AttackCoolDown);
        }

        public void Attack(int itemIndex)
        {
            if (!CanAttack(itemIndex)) return;

            // Enable new item, disable old
            if (_currentEquipment != _equipmentList[itemIndex])
            {
                _currentEquipment.gameObject.SetActive(false);
                _currentEquipment = _equipmentList[itemIndex];
                _currentEquipment.gameObject.SetActive(true);
                _attackCooldownTimer.Reset(_currentEquipment.AttackCoolDown);
            }
            
            _attackCooldownTimer.Start();
            _currentEquipment.Use(_playerController.AnimatorComp);
        }

        public bool CanAttack(int itemIndex)
        {
            return !IsAttacking() 
                   && itemIndex <= _equipmentList.Count - 1
                   && _equipmentList[itemIndex].IsUsable();
        }

        public bool IsAttacking() => _attackCooldownTimer.IsRunning;

        private void Update()
        {
            _attackCooldownTimer.Tick(Time.deltaTime);
        }

        public void ToggleShield(bool isUsingShield)
        {
            if (isUsingShield)
            {
                if (_currentEquipment != null)
                    _currentEquipment.gameObject.SetActive(false);
                _shield.gameObject.SetActive(true);
                _shield.Use(_playerController.AnimatorComp);
            }
            else
            {
                if (_currentEquipment != null)
                    _currentEquipment.gameObject.SetActive(true);
                _shield.gameObject.SetActive(false);
                _shield.DisableShield(_playerController.AnimatorComp);
            }
        }
    }
}