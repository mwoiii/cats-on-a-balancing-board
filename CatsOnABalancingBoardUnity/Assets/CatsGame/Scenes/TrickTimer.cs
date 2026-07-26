using System;
using System.Collections;
using UnityEngine;

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

    void Awake()
    {
        ResetTimerAndCombo();
    }

    void Start() {
        StartCoroutine(Timer());
        CatManagerScript.LostCat += ResetTimerAndCombo;

        countdownSource.volume = volume / 1.5f;
        completeSource.volume = volume;
    }

    public static int currentSecond { get; private set; }

    public static int litterBonus { get; private set; }

    public static event Action<int> onTimerChanged;

    public static event Action<int> onLitterBonusChanged;

    IEnumerator Timer() {
        while (GameLogicScript.gameRunning) {
            yield return new WaitForSeconds(1);
            currentSecond--;
            onTimerChanged?.Invoke(currentSecond);
            if (currentSecond == 3)
            {
                PlayCountdown();
            }
            if (currentSecond == 0) {
                PlayComplete();

                StartCoroutine(CatSpawnerScript.instance.PopulateBoard(litterBonus));
                litterBonus *= 2;
                onLitterBonusChanged?.Invoke(litterBonus);
                currentSecond = trickLength;
            }
        }
    }

    void ResetTimerAndCombo() {
        currentSecond = trickLength;
        litterBonus = initialBonus;
        onTimerChanged?.Invoke(currentSecond);
        onLitterBonusChanged?.Invoke(litterBonus);

        countdownSource.Stop();
        completeSource.pitch = basePitch;
    }

    void OnDestroy() {
        CatManagerScript.LostCat -= ResetTimerAndCombo;
    }

    void PlayCountdown()
    {
        if (countdownSound != null && countdownSource != null)
        {
            countdownSource.PlayOneShot(countdownSound);
        }
    }

    void PlayComplete()
    {
        if (completeSound != null && completeSource != null)
        {
            completeSource.PlayOneShot(completeSound);
            completeSource.pitch = Mathf.Min(completeSource.pitch + pitchStep, maxPitch);
        }
    }
}
