using OMC.ECS;
using System;
using System.Collections;
using UnityEngine;

namespace OMC {
    public class TrickTimer : MonoBehaviour {
        public int trickLength;

        float bonusBase;
        float bonusMult;

        public AudioClip countdownSound;

        public AudioClip completeSound;

        public AudioSource countdownSource;

        public AudioSource completeSource;

        public float volume = 0.7f;

        public float basePitch = 1f;

        public float pitchStep = 0.1f;

        public float maxPitch = 2f;

        private int comboCounter = 0;

        public static int currentSecond { get; private set; }

        public static int litterBonus { get; private set; }

        public static event Action<int> OnTimerChanged;

        public static event Action<int> OnLitterBonusChanged;

        private void Start() {
            ResetTimerAndCombo(0);
            WeightDropper.FirstWeightDropped += GetStartedWithIt;
            CatManagerScript.LostCat += ResetTimerAndCombo;
            CatCountBridgingSystem.CatCountChange += ResetTimerAndCombo;

            countdownSource.volume = volume / 1.5f;
            completeSource.volume = volume;
        }

        private void GetStartedWithIt() {
            StartCoroutine(Timer());
        }

        private IEnumerator Timer() {
            while (GameLogicScript.gameRunning) {
                yield return new WaitForSeconds(1);
                DecrementTimer();
                if (currentSecond == 3) {
                    PlayCountdown();
                }
                if (currentSecond == 0) {
                    PlayComplete();

                    if (CatSpawnerScript.instance) {
                        StartCoroutine(CatSpawnerScript.instance.PopulateBoard(litterBonus));

                        IncrementLitterBonus();
                        ResetTimer();
                    } else {
                        CatSpawnRequest.Enqueue(litterBonus);

                        IncrementLitterBonus();
                        ResetTimer();
                    }
                }
            }
        }

        private void ResetTimerAndCombo(int count) {
            if (count <= 0) {
                ResetLitterBonus();
                ResetTimer();
            }
        }

        private void ResetLitterBonus() {
            bonusMult = 10;
            bonusBase = 1;
            foreach (WeightDef def in WeightDropper.instance.currentWeightRotation) {
                bonusMult += def.multAdd;
                bonusBase += def.baseAdd;
            }
            bonusMult = Mathf.Max(bonusMult, 0);
            bonusBase = Mathf.Max(bonusBase, 1);
            Debug.Log($"{bonusMult} * {bonusBase} ^ k");

            litterBonus = (int)Mathf.Floor(bonusMult);
            OnLitterBonusChanged?.Invoke(litterBonus);
        }

        private void IncrementLitterBonus() {
            comboCounter++;
            litterBonus = (int)Mathf.Ceil(bonusMult * Mathf.Pow(bonusBase, comboCounter));
            OnLitterBonusChanged?.Invoke(litterBonus);
        }

        private void ResetTimer() {
            currentSecond = trickLength;
            OnTimerChanged?.Invoke(currentSecond);

            countdownSource.Stop();
            completeSource.pitch = basePitch;
        }

        private void DecrementTimer() {
            currentSecond--;
            OnTimerChanged?.Invoke(currentSecond);
        }

        private void OnDestroy() {
            CatManagerScript.LostCat -= ResetTimerAndCombo;
            CatCountBridgingSystem.CatCountChange -= ResetTimerAndCombo;
            WeightDropper.FirstWeightDropped -= GetStartedWithIt;
        }

        private void PlayCountdown() {
            if (countdownSound && countdownSource) {
                countdownSource.PlayOneShot(countdownSound);
            }
        }

        private void PlayComplete() {
            if (completeSound && completeSource) {
                completeSource.PlayOneShot(completeSound);
                completeSource.pitch = Mathf.Min(completeSource.pitch + pitchStep, maxPitch);
            }
        }
    }
}
