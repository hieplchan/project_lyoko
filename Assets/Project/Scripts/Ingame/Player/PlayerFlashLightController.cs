using System;
using KBCore.Refs;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerFlashLightController : ValidatedMonoBehaviour
    {
        [SerializeField, Child] private Light _flashLight;
        [SerializeField, Anywhere] private InputReader _input;

        private void Awake()
        {
            _input.Attack += ToggleLight;
        }

        private void OnDestroy()
        {
            _input.Attack -= ToggleLight;
        }

        private void ToggleLight()
        {
            _flashLight.gameObject.SetActive(!_flashLight.gameObject.activeSelf);
        }

        [Button]
        private void TurnOnLight() => _flashLight.gameObject.SetActive(true);
        
        [Button]
        private void TurnOffLight() => _flashLight.gameObject.SetActive(false);
    }
}