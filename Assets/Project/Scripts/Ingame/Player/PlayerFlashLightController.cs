using KBCore.Refs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerFlashLightController : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private Light _flashLight;
        
        [Button]
        private void TurnOnLight() => _flashLight.gameObject.SetActive(true);
        
        [Button]
        private void TurnOffLight() => _flashLight.gameObject.SetActive(false);
    }
}