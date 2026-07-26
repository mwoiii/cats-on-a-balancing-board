using System.Collections;
using UnityEngine;

public class TrickTimer : MonoBehaviour
{
    public int trickLength;
    public int initialBonus;

    WaitForSeconds second = new(1);
    
    void Start()
    {
        ResetTimerAndCombo();
        StartCoroutine(Timer());
        CatManagerScript.LostCat += ResetTimerAndCombo;
    }

    public static int currentSecond {get; private set;}

    public static int litterBonus {get; private set;}

    IEnumerator Timer()
    {
        while (true)
        {
            yield return second;
            currentSecond--;
            if (currentSecond == 0)
            {
                StartCoroutine(CatSpawnerScript.instance.PopulateBoard(litterBonus));
                litterBonus *= 2;
                currentSecond = trickLength;
            }   
        }
    }

    void ResetTimerAndCombo()
    {
        currentSecond = trickLength;
        litterBonus = initialBonus;
    }

    void OnDestroy()
    {
        CatManagerScript.LostCat -= ResetTimerAndCombo;
    }
}
