using UnityEngine.UI;
using OMC;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BonusTimerRadial : MonoBehaviour
{
    public Image radialImage;
    float cycleStartTime;

    void Start()
    {
        TrickTimer.OnTimerChanged += OnTimerChanged;
        cycleStartTime = Time.time;
    }

    void OnDestroy()
    {
        TrickTimer.OnTimerChanged -= OnTimerChanged;
    }

    void OnTimerChanged(int seconds)
    {
        if (TrickTimer.instance && seconds == TrickTimer.instance.trickLength - TrickTimer.instance.shorterTricksPoints)
        {
            cycleStartTime = Time.time;
        }
    }

    void Update()
    {
        if (!radialImage || !TrickTimer.instance || !GameLogicScript.firstWeightDropped)
        {
            return;
        }

        float elapsed = Time.time - cycleStartTime;
        radialImage.fillAmount = 1 - Mathf.Clamp01(elapsed/TrickTimer.instance.trickLength);
    }
}
