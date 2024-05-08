using System;
using KBCore.Refs;
using StartledSeal.Utils;
using SuperMaxim.Messaging;
using UnityEngine;

namespace StartledSeal
{
    public class Collectible : Entity
    {
        [SerializeField] private int _score;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Const.PlayerTag))
            {
                var payload = new PlayerAddedScorePayload()
                {
                    AddedScore = _score
                };
                Messenger.Default.Publish(payload);

                Destroy(gameObject);
            }
        }
    }
}