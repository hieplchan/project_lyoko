using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StartledSeal
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [SerializeField] private float _spawnForce = 1000f;
        [SerializeField] private int _spawnDelay = 100;
        
        [SerializeField] private bool _spawnWhenStartGame;
        [SerializeField] private int _spawnWhenStartGameQuantity = 5;
        
        [SerializeField] private List<Transform> _spawnPoints;
        [SerializeField] private List<GameObject> _enemyPrefabs;

        private void Start()
        {
            if (_spawnWhenStartGame)
            SpawnEnemy(_spawnWhenStartGameQuantity);
        }

        [Button]
        private async UniTask SpawnEnemy(int quantity = 100)
        {
            for (int i = 0; i < quantity; i++)
            {
                GameObject enemy = GameObject.Instantiate(_enemyPrefabs[i % _enemyPrefabs.Count], 
                    _spawnPoints[i % _spawnPoints.Count].position, 
                    _spawnPoints[i % _spawnPoints.Count].rotation);
                enemy.GetComponent<NormalEnemy>().RigidbodyComp.AddRelativeForce(Random.onUnitSphere * _spawnForce);

                await UniTask.Delay(_spawnDelay);
            }
        }
    }
}