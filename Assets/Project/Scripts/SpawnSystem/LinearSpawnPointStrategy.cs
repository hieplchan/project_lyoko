using UnityEngine;

namespace StartledSeal
{
    public class LinearSpawnPointStrategy : ISpawnPointStrategy
    {
        private Transform[] _spawnPoints;
        private int _index = 0;

        public LinearSpawnPointStrategy(Transform[] spawnPoints)
        {
            _spawnPoints = spawnPoints;
        }
        
        public Transform NextSpawnPoint()
        {
            Transform result = _spawnPoints[_index];
            _index = (_index + 1) % _spawnPoints.Length;
            return result;
        }
    }
}