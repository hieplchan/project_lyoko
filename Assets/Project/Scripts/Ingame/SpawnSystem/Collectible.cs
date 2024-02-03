using System;
using KBCore.Refs;
using StartledSeal.Utils;
using UnityEngine;

namespace StartledSeal
{
    public class Collectible : Entity
    {
        [SerializeField] private int _score;
        [SerializeField] private IntEventChannel _scoreChannel;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Const.PlayerTag))
            {
                _scoreChannel.Invoke(_score);
                Destroy(gameObject);
            }
        }
    }
}