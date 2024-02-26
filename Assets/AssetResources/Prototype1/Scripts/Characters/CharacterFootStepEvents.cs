using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CharacterFootStepEvents : MonoBehaviour
{
    [SerializeField] Character _character;
    [SerializeField] AudioClip[] _sounds;

    public void PlaySound()
    {
        _character.SoundController.PlaySound(_sounds[Random.Range(0, _sounds.Length - 1)]);
    }
}
