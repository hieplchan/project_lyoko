using Cysharp.Threading.Tasks;
using KBCore.Refs;
using UnityEngine;

namespace StartledSeal
{
    public abstract class BaseEquipment : ValidatedMonoBehaviour
    {
        [SerializeField, Anywhere] public PlayerWeaponController _weaponController;
        
        public abstract bool IsUsable();
        public abstract UniTask Use();
    }
}