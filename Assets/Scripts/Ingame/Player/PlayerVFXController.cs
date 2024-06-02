using System;
using System.Collections.Generic;
using BrunoMikoski.AnimationSequencer;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using StartledSeal.Common;
using UnityEngine;
using UnityEngine.Rendering;

namespace StartledSeal.Ingame.Player
{
    public sealed class PlayerVFXController : SerializedMonoBehaviour
    {
        [SerializeField] private PlayerController _player;
        [SerializeField] private Dictionary<string, ParticleSystem> _vfxDic;
        
        [Header("Run VFX")]
        [SerializeField] private AnimationSequencerController _runVFX;
        [SerializeField] private float _delay = 1f;
        
        private void Start()
        {
            CheckPlayRunVFX().Forget();
        }

        private async UniTask CheckPlayRunVFX()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_delay));
            if (_player.GetStateHash() == Const.LocomotionHash)
                _runVFX.Play();

            await CheckPlayRunVFX();
        }

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