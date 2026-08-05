using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace OMC {
    public class AudioPool : MonoBehaviour {
        public static AudioPool instance;

        public AudioClip explosionSound;

        public AudioClip supernovaSound;

        public float volume = 0.3f;

        public float minPitch = 0.9f;

        public float maxPitch = 1.1f;

        public int maxConcurrentSounds = 100;

        private ObjectPool<AudioSource> sourcePool;

        [System.NonSerialized]
        public Queue<AudioSource> sourceQueue;

        private void Awake() {
            instance = this;
            sourcePool = new ObjectPool<AudioSource>(CreateNewAudioSource, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, true, maxConcurrentSounds, maxConcurrentSounds);
            sourceQueue = new Queue<AudioSource>(maxConcurrentSounds);
        }

        private AudioSource CreateNewAudioSource() {
            GameObject audioObject = new($"PooledAudioSource");
            AudioSource audioSource = audioObject.AddComponent<AudioSource>();
            ReturnToAudioPool returnToPool = audioObject.AddComponent<ReturnToAudioPool>();
            returnToPool.audioSource = audioSource;
            returnToPool.pool = sourcePool;
            audioSource.spatialBlend = 1;
            audioSource.playOnAwake = false;
            audioObject.transform.SetParent(transform);
            return audioSource;
        }

        private void OnTakeFromPool(AudioSource audioSource) {
            audioSource.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(AudioSource audioSource) {
            audioSource.gameObject.SetActive(false);
            sourceQueue.TryDequeue(out audioSource);
        }

        private void OnDestroyPoolObject(AudioSource audioSource) {
            Destroy(audioSource.gameObject);
        }


        void PlaySoundAt(AudioClip clip, Vector3 pos) {
            if (clip == null) {
                return;
            }

            AudioSource audioSource;
            if (sourcePool.CountActive >= maxConcurrentSounds) {
                sourceQueue.TryDequeue(out audioSource);
                if (!audioSource) {
                    return;
                }
            } else {
                audioSource = sourcePool.Get();
            }

            audioSource.Stop();
            audioSource.transform.position = pos;
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.pitch = Random.Range(minPitch, maxPitch);
            audioSource.GetComponent<ReturnToAudioPool>().lifetime = GetLifetime(audioSource);
            audioSource.Play();

            sourceQueue.Enqueue(audioSource);
        }

        private float GetLifetime(AudioSource audioSource) {
            float lifetime = (audioSource.clip.length - audioSource.time) / audioSource.pitch;
            if (audioSource.pitch < 0f) {
                lifetime = audioSource.clip.length + lifetime;
            }
            return lifetime;
        }

        public void PlayExplosionSoundAt(Vector3 position) {
            PlaySoundAt(explosionSound, position);
        }

        public void PlaySupernovaSoundAt(Vector3 position) {
            PlaySoundAt(supernovaSound, position);
        }
    }
}
