using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CharacterSoundController : MonoBehaviour
{
    [SerializeField] private AudioSource _characterAudioSource;

    public AudioSource CharacterAudioSource => _characterAudioSource;

    public void PlaySound(AudioClip audioClip)
    {
        _characterAudioSource.PlayOneShot(audioClip);
    }
}
