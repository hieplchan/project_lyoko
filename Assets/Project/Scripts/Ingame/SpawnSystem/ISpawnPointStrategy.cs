using UnityEngine;

namespace StartledSeal
{
    public interface ISpawnPointStrategy
    {
        public Transform NextSpawnPoint();
    }
}