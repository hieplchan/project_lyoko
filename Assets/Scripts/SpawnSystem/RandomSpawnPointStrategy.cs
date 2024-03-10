using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StartledSeal
{
    public class RandomSpawnPointStrategy : ISpawnPointStrategy
    {
        private Transform[] _spawnPoints;
        private List<Transform> _unusedSpawnPoints;

        public RandomSpawnPointStrategy(Transform[] spawnPoints)
        {
            _spawnPoints = spawnPoints;
            _unusedSpawnPoints = new List<Transform>();
        }
        
        public Transform NextSpawnPoint()
        {
            if (!_unusedSpawnPoints.Any())
                _unusedSpawnPoints = new List<Transform>(_spawnPoints.ToList());

            int index = Random.Range(0, _unusedSpawnPoints.Count);
            Transform result = _unusedSpawnPoints[index];
            _unusedSpawnPoints.RemoveAt(index);
            return result;
        }
    }
}