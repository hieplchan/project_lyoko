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
        [SerializeField] private List<BaseEquipment> _equipmentList;
        [SerializeField] private Shield _shield;
        
        public BaseEquipment CurrentEquipment => _equipmentList[_currentEquipmentIndex];
        private int _currentEquipmentIndex;
        
        public bool IsUsingShield { get; private set; }
        public bool IsAttacking => CurrentEquipment.CurrentState != EquipmentState.NotBeingUsed;
        
        private void Awake()
        {
            _currentEquipmentIndex = 0;
        }
        
        private void Update()
        {
            if (CurrentEquipment != null)
                CurrentEquipment.Update();
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
            if (_equipmentList.Count == 0) return;
            if (itemIndex < 0 || itemIndex > _equipmentList.Count - 1) return;

            if (active)
                CheckChangeEquipment(itemIndex);

            if (CurrentEquipment != null)
                CurrentEquipment.Use(active);
        }
        
        private void CheckChangeEquipment(int itemIndex)
        {
            if (CurrentEquipment != _equipmentList[itemIndex])
            {
                CurrentEquipment.StopUsing();
                CurrentEquipment.gameObject.SetActive(false);
                
                _currentEquipmentIndex = itemIndex;
                CurrentEquipment.gameObject.SetActive(true);
            }
        }

        public void OnToggleShield(bool isUsingShield)
        {
            // MLog.Debug("PlayerWeaponController", $"PlayerWeaponController isUsingShield: {isUsingShield}");
            PlayerControllerComp.IsRotationLocked = IsUsingShield = isUsingShield;
            
            if (IsUsingShield)
            {
                _shield.gameObject.SetActive(true);
                _shield.NormalAttack();
                PlayerControllerComp.IsForcedWalking = true;
            }
            else
            {
                _shield.gameObject.SetActive(false);
                _shield.DisableShield();
                PlayerControllerComp.IsForcedWalking = false;
            }
        }
    }
}