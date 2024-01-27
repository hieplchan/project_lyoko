using UnityEngine;

namespace StartledSeal
{
    public class EntityFactory<T> : IEntityFactory<T> where T : Entity
    {
        private EntityData[] _data;

        public EntityFactory(EntityData[] data)
        {
            _data = data;
        }
        
        public T Create(Transform spawnPoint)
        {
            EntityData data = _data[Random.Range(0, _data.Length)];
            GameObject instance = GameObject.Instantiate(data.prefab, spawnPoint.position, spawnPoint.rotation);
            return instance.GetComponent<T>();
        }
    }
}