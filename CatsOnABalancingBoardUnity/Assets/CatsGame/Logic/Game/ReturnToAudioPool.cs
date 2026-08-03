using UnityEngine;
using UnityEngine.Pool;

namespace Assets.CatsGame.Logic.Game {
    [RequireComponent(typeof(AudioSource))]
    public class ReturnToAudioPool : MonoBehaviour {
        public AudioSource audioSource;

        public IObjectPool<AudioSource> pool;

        public float lifetime;

        private void Start() {
            audioSource = GetComponent<AudioSource>();
        }

        private void Update() {
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f) {
                pool.Release(audioSource);
            }
        }
    }
}
