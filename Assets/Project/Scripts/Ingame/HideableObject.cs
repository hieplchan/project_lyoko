using System;
using StartledSeal.Common;
using UnityEngine;

namespace StartledSeal
{
    public class HideableObject : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject _guildGameObject;
        
        private PlayerController _player;

        public bool CanInteract() => _player != null;

        public void Interact()
        {
            // noop
        }

        private void OnTriggerEnter(Collider other)
        {
            MLog.Debug("HideableObject", "OnTriggerEnter");
            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                _player = player;
                if (_guildGameObject != null) _guildGameObject.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            MLog.Debug("HideableObject", "OnTriggerExit");
            if (other.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                _player = null;
                if (_guildGameObject != null) _guildGameObject.SetActive(false);
            }
        }
    }
}