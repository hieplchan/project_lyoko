using System;
using SuperMaxim.Messaging;
using UnityEngine;
using Random = System.Random;

namespace StartledSeal
{
    public class CollectiblesSpawnManager : MonoBehaviour
    {
        [SerializeField] private CollectibleData[] collectibleData;
        [SerializeField] private float spawnPercentage = 0.3f;
        
        private EntityFactory<Collectible> _entityFactory;
        
        private void Awake()
        {
            _entityFactory = new EntityFactory<Collectible>(collectibleData);

            Messenger.Default.Subscribe<SpawnCollectibleRequest>(CheckToSpawnCollectibles);
        }

        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<SpawnCollectibleRequest>(CheckToSpawnCollectibles);
        }

        private void CheckToSpawnCollectibles(SpawnCollectibleRequest payload)
        {
            if (UnityEngine.Random.Range(0f, 1f) < spawnPercentage)
            {
                var spawnPoint = payload.spawnTransform;
                var vector3 = spawnPoint.position;
                vector3.y = 0.5f;
                spawnPoint.position = vector3;
                
                _entityFactory.Create(spawnPoint);
            }
        }
    }
}