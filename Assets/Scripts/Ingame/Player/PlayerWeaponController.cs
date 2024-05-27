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
        [field: SerializeField, Parent] public PlayerController PlayerControllerComp { get; private set; }
        [SerializeField, Anywhere] private InputReader _input;

        [SerializeField] private Transform _equipmentSpawnPoint;
        [SerializeField] private Shield _shield;
        [SerializeField] private List<BaseEquipment> _equipmentList;
        
        public bool IsUsingShield { get; private set; }

        public bool IsAttacking => _attackCooldownTimer.IsRunning;

        private CooldownTimer _attackCooldownTimer;
        private BaseEquipment _currentEquipment;
        
        private void Awake()
        {
            _currentEquipment = _equipmentList[0];
            _attackCooldownTimer = new CooldownTimer(_currentEquipment.AttackCoolDown);
            _attackCooldownTimer.Reset(_currentEquipment.AttackCoolDown);
        }
        
        public void EnableUsingItem()
        {
            _input.Item1 += OnItem1;
            _input.Item2 += OnItem2;
            _input.Item3 += OnItem3;
            _input.Shield += OnToggleShield;
        }
        
        public void DisableUsingItem()
        {
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
                if (!IsAttacking && CanAttack(itemIndex))
                    Attack(itemIndex);
            }
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
            _currentEquipment.NormalAttack(PlayerControllerComp.AnimatorComp);
        }

        public bool CanAttack(int itemIndex)
        {
            return !IsAttacking
                   && itemIndex <= _equipmentList.Count - 1
                   && _equipmentList[itemIndex].IsUsable();
        }
        
        private void Update()
        {
            _attackCooldownTimer.Tick(Time.deltaTime);
        }

        public void OnToggleShield(bool isUsingShield)
        {
            MLog.Debug("PlayerWeaponController", $"PlayerWeaponController isUsingShield: {isUsingShield}");
            PlayerControllerComp.IsRotationLocked = IsUsingShield = isUsingShield;
            
            if (IsUsingShield)
            {
                _shield.gameObject.SetActive(true);
                _shield.NormalAttack(PlayerControllerComp.AnimatorComp);
            }
            else
            {
                _shield.gameObject.SetActive(false);
                _shield.DisableShield(PlayerControllerComp.AnimatorComp);
            }
        }
    }
}