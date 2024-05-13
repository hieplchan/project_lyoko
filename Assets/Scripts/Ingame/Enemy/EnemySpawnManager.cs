using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StartledSeal
{
    public class EnemySpawnManager : MonoBehaviour
    {
        [SerializeField] private float _spawnForce = 1000f;
        [SerializeField] private int _spawnDelay = 100;

        [SerializeField] private List<Transform> _spawnPoints;
        [SerializeField] private List<GameObject> _enemyPrefabs;

        [Button]
        private async UniTask SpawnEnemy(int quantity = 10)
        {
            for (int i = 0; i < quantity; i++)
            {
                GameObject enemy = GameObject.Instantiate(_enemyPrefabs[i % _enemyPrefabs.Count], 
                    _spawnPoints[i % _spawnPoints.Count].position, 
                    _spawnPoints[i % _spawnPoints.Count].rotation);
                enemy.GetComponent<Enemy>().RigidbodyComp.AddRelativeForce(Random.onUnitSphere * _spawnForce);

                await UniTask.Delay(_spawnDelay);
            }
        }
    }
}