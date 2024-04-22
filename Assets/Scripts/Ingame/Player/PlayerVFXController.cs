using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace StartledSeal.Ingame.Player
{
    public sealed class PlayerVFXController : SerializedMonoBehaviour
    {
        [SerializeField] private Dictionary<string, ParticleSystem> _vfxDic;

        public void PlayVFX(string stateHash)
        {
            GetVFX(stateHash)!.Play();
        }

        public void StopVFX(string stateHash)
        {
            GetVFX(stateHash)!.Stop();
        }

        public void RestartVFX(string stateHash)
        {
            var vfx = GetVFX(stateHash);
            vfx!.Simulate(0f, true, true);
            vfx.Play();
        }

        private ParticleSystem GetVFX(string stateHash) => _vfxDic[stateHash];
    }
}