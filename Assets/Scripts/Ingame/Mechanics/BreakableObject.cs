using System;
using BrunoMikoski.AnimationSequencer;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace StartledSeal
{
    public class BreakableObject: MonoBehaviour, IDamageable
    {
        [SerializeField] private ParticleSystem _vfx;
        [SerializeField] private GameObject _model;
        [SerializeField] private AnimationSequencerController _animationSequencerController;
        
        [Button]
        public void TakeDamage(int damageAmount)
        {
            _animationSequencerController!.Play();
            _vfx!.Play();
            _model.gameObject.SetActive(false);
        }
    }
}