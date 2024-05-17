using System;
using System.Collections.Generic;
using KBCore.Refs;
using StartledSeal.Common;
using SuperMaxim.Messaging;
using UnityEngine;

namespace StartledSeal
{
    [RequireComponent(typeof(SphereCollider))]
    public class NearbyEnemyDetector : ValidatedMonoBehaviour
    { 
        [SerializeField, Self] private SphereCollider _collider;
        [SerializeField] private float _publishMessageInterval = 0.1f;
        [SerializeField] private float _warningDistanceThreshold = 15f;
        
        private List<NormalEnemy> _nearbyEnemyList;
        private float _lastPublishMessageTime;

        private void Awake()
        {
            _lastPublishMessageTime = Time.time;
            _nearbyEnemyList ??= new List<NormalEnemy>();
        }

        private void Update()
        {
            CheckToPublishMessage();
        }

        private void CheckToPublishMessage()
        {
            if (_nearbyEnemyList.Count == 0) return;
            if (Time.time < _lastPublishMessageTime + _publishMessageInterval) return;

            PublishDistanceMessage();
        }

        private void PublishDistanceMessage()
        {
            _lastPublishMessageTime = Time.time;
            
            var distance = GetNearestEnemyDistance();
            float warningRatio = Mathf.Clamp(1 - distance/_warningDistanceThreshold, 0, 1);
            
            var payload = new NearbyEnemyDistancePayload()
            {
                NearestEnemyMinDistance = distance,
                WarningRatio = warningRatio
            };
            Messenger.Default.Publish(payload);
            MLog.Debug($"PublishDistanceMessage {payload.NearestEnemyMinDistance} - {payload.WarningRatio}");
        }

        public float GetNearestEnemyDistance()
        {
            if (_nearbyEnemyList.Count == 0) 
                return Const.NoNearbyEnemyDistance;

            var minDistance = Const.NoNearbyEnemyDistance;
            foreach (var enemy in _nearbyEnemyList)
            {
                var distance = Vector3.Distance(enemy.transform.position, transform.position);
                if (distance < minDistance)
                    minDistance = distance;
            }

            return minDistance;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<NormalEnemy>(out var enemy))
            {
                _nearbyEnemyList.Add(enemy);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent<NormalEnemy>(out var enemy))
            {
                if (_nearbyEnemyList.Contains(enemy))
                    _nearbyEnemyList.Remove(enemy);
            }
            
            if (_nearbyEnemyList.Count == 0)
                PublishDistanceMessage();
        }
    }
}