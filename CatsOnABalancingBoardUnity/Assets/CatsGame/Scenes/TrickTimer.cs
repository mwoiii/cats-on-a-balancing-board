using System;
using System.Collections;
using UnityEngine;

public class TrickTimer : MonoBehaviour {
    public int trickLength;
    public int initialBonus;

    void Start() {
        ResetTimerAndCombo();
        StartCoroutine(Timer());
        CatManagerScript.LostCat += ResetTimerAndCombo;
    }

    public static int currentSecond { get; private set; }

    public static int litterBonus { get; private set; }

    public static event Action<int> onTimerChanged;

    public static event Action<int> onLitterBonusChanged;

    IEnumerator Timer() {
        while (true) {
            yield return new WaitForSeconds(1);
            currentSecond--;
            onTimerChanged?.Invoke(currentSecond);
            if (currentSecond == 0) {
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
    }

    void OnDestroy() {
        CatManagerScript.LostCat -= ResetTimerAndCombo;
    }
}
