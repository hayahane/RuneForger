using System.Collections.Generic;
using UnityEngine;

namespace RuneForger.SoundFX
{
    public class CharacterVoice : MonoBehaviour
    {
        [SerializeField]
        private AudioClip[] _clips;
        private AudioSource _audioSource;

        private readonly Dictionary<string, int> _clipIndex = new()
        {
            { "Jump", 0 },
            { "Attack1", 1 },
            { "Attack2", 2 },
            { "Hit", 3 },
            { "Death", 4 }
        };
        
        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        public void PlayVoice(string clipName)
        {
            if (_clipIndex.TryGetValue(clipName, out var index))
            {
                _audioSource.clip = _clips[index];
                _audioSource.Play();
            }
        }
    }
}