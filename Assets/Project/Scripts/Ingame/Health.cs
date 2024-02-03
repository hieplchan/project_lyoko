using System;
using UnityEngine;

namespace StartledSeal
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _maxHealth;
        [SerializeField] private FloatEventChannel _playerHealthEventChanel;

        private float _currentHealth;

        private void Awake()
        {
            _currentHealth = _maxHealth;
        }

        private void Start()
        {
            PublishHealthPercentage();
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;
            PublishHealthPercentage();
        }
        
        private void PublishHealthPercentage()
        {
            if (_playerHealthEventChanel != null) 
                _playerHealthEventChanel.Invoke(_currentHealth/_maxHealth);
        }
    }
}