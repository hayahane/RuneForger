using UnityEngine;

namespace RuneForger.SoundFX
{
    public class RandomSFX : MonoBehaviour
    {
        [SerializeField]
        private AudioClip[] _clips;
        private AudioSource _audioSource;
        [SerializeField]
        private float _interval = 0.5f;
        private float _timer = 0f;
        [SerializeField]
        private bool _is_continous = false;

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            _timer = _interval;
        }

        private void Update()
        {
            if (!_is_continous)
            {
                return;
            }
            if (_clips.Length == 0)
            {
                return;
            }

            _timer -= Time.deltaTime;
            if (_timer > 0f) return;
            PlayRandomly();
            
            _timer = _interval;
        }

        public void PlayRandomly()
        {
            _audioSource.clip = _clips[Random.Range(0, _clips.Length)];
            _audioSource.Play();
        }
    }
}