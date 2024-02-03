using System;
using Cysharp.Threading.Tasks.Triggers;
using StartledSeal.Utils;
using UnityEngine;

namespace StartledSeal
{
    public class CollectibleSpawnManager : EntitySpawnManager
    {
        [SerializeField] private CollectibleData[] collectibleDatas;
        [SerializeField] private float _spawnInterval = 1f;

        private EntitySpawner<Collectible> _spawner;
        private int _counter;
        
        private CooldownTimer _spawnTimer;

        protected override void Awake()
        {
            base.Awake();

            _spawner = new EntitySpawner<Collectible>(
                new EntityFactory<Collectible>(collectibleDatas),
                spawnPointStrategy);
            
            _spawnTimer = new CooldownTimer(_spawnInterval);
            _spawnTimer.OnTimerStop += () =>
            {
                if (_counter >= spawnPoints.Length)
                {
                    _spawnTimer.Stop();
                    return;
                }
                _spawnTimer.Start();
                Spawn();
            };
        }

        private void Start() => _spawnTimer.Start();

        private void Update() => _spawnTimer.Tick(Time.deltaTime);

        public override void Spawn()
        {
            _spawner.Spawn();
            _counter++;
        }
    }
}