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

        [SerializeField] private List<BaseEquipment> _equipmentList;

        private CooldownTimer _attackCooldownTimer;
        private BaseEquipment _currentEquipment;

        private void Awake()
        {
            _currentEquipment = _equipmentList[0];
            _attackCooldownTimer = new CooldownTimer(_currentEquipment.AttackCoolDown);
        }

        public void Attack()
        {
            if (!CanAttack()) return;
            
            _attackCooldownTimer.Reset(_currentEquipment.AttackCoolDown);
            _attackCooldownTimer.Start();
            _currentEquipment.Use(_playerController.AnimatorComp);
        }

        public bool CanAttack()
        {
            return !IsAttacking() && _currentEquipment.IsUsable();
        }

        public bool IsAttacking()
        {
            MLog.Debug("PlayerWeaponController", $"IsAttacking {_attackCooldownTimer.IsRunning}");
            
            return _attackCooldownTimer.IsRunning;
        }

        private void Update()
        {
            _attackCooldownTimer.Tick(Time.deltaTime);
        }
    }
}