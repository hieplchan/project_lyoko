using System;
using System.Collections.Generic;
using KBCore.Refs;
using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerWeaponController : ValidatedMonoBehaviour
    {
        [SerializeField] private Transform _equipmentSpawnPoint;
        [SerializeField, Parent] private PlayerController _playerController;

        [SerializeField] private List<BaseEquipment> _equipmentList;
        
        public void Attack()
        {
            _equipmentList[0].Use();
        }
    }
}