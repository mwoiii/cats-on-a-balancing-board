using System.Collections;
using UnityEngine;

public class TrickTimer : MonoBehaviour
{
    public int trickLength;
    public int catCount;

    WaitForSeconds second = new(1);
    
    void Start()
    {
        currentSecond = trickLength;
        StartCoroutine(Timer());
        CatManagerScript.LostCat += ResetTimer;
    }

    public static int currentSecond {get; private set;}
    IEnumerator Timer()
    {
        while (true)
        {
            yield return second;
            currentSecond--;
            if (currentSecond == 0)
            {
                StartCoroutine(CatSpawnerScript.instance.PopulateBoard(catCount));
                currentSecond = trickLength;
            }   
        }
    }

    void ResetTimer()
    {
        currentSecond = 0;
    }

    void OnDestroy()
    {
        CatManagerScript.LostCat -= ResetTimer;
    }
}
