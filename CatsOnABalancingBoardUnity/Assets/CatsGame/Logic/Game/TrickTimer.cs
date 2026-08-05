using OMC.ECS;
using System;
using System.Collections;
using UnityEngine;

namespace OMC {
    public class TrickTimer : MonoBehaviour {
        public int trickLength;

        public int initialBonus;

        public AudioClip countdownSound;

        public AudioClip completeSound;

        public AudioSource countdownSource;

        public AudioSource completeSource;

        public float volume = 0.7f;

        public float basePitch = 1f;

        public float pitchStep = 0.1f;

        public float maxPitch = 2f;

        private void Awake() {
            ResetTimerAndCombo(0);
        }

        private void Start() {
            WeightDropper.FirstWeightDropped += GetStartedWithIt;
            CatManagerScript.LostCat += ResetTimerAndCombo;
            CatCountBridgingSystem.CatCountChange += ResetTimerAndCombo;

            countdownSource.volume = volume / 1.5f;
            completeSource.volume = volume;
        }

        private void GetStartedWithIt() {
            StartCoroutine(Timer());
        }

        public static int currentSecond { get; private set; }

        public static int litterBonus { get; private set; }

        public static event Action<int> OnTimerChanged;

        public static event Action<int> OnLitterBonusChanged;

        private IEnumerator Timer() {
            while (GameLogicScript.gameRunning) {
                yield return new WaitForSeconds(1);
                currentSecond--;
                OnTimerChanged?.Invoke(currentSecond);
                if (currentSecond == 3) {
                    PlayCountdown();
                }
                if (currentSecond == 0) {
                    PlayComplete();

                    if (CatSpawnerScript.instance != null) {
                        StartCoroutine(CatSpawnerScript.instance.PopulateBoard(litterBonus));
                        litterBonus *= 2;
                        OnLitterBonusChanged?.Invoke(litterBonus);
                        currentSecond = trickLength;
                    } else {
                        CatSpawnRequest.Enqueue(litterBonus);
                        litterBonus *= 2;
                        OnLitterBonusChanged?.Invoke(litterBonus);
                        currentSecond = trickLength;
                    }
                }
            }
        }

        private void ResetTimerAndCombo(int count) {
            if (count >= 0) {
                return;
            }

            currentSecond = trickLength;
            litterBonus = initialBonus;
            OnTimerChanged?.Invoke(currentSecond);
            OnLitterBonusChanged?.Invoke(litterBonus);

            countdownSource.Stop();
            completeSource.pitch = basePitch;
        }

        private void OnDestroy() {
            CatManagerScript.LostCat -= ResetTimerAndCombo;
            CatCountBridgingSystem.CatCountChange -= ResetTimerAndCombo;
            WeightDropper.FirstWeightDropped -= GetStartedWithIt;
        }

        private void PlayCountdown() {
            if (countdownSound != null && countdownSource != null) {
                countdownSource.PlayOneShot(countdownSound);
            }
        }

        private void PlayComplete() {
            if (completeSound != null && completeSource != null) {
                completeSource.PlayOneShot(completeSound);
                completeSource.pitch = Mathf.Min(completeSource.pitch + pitchStep, maxPitch);
            }
        }
    }
}
