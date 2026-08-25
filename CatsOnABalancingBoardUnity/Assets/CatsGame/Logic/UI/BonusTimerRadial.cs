using UnityEngine.UI;
using OMC;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BonusTimerRadial : MonoBehaviour
{
    public Image radialImage;
    float cycleStartTime;
    bool started = false;

    void Start()
    {
        TrickTimer.OnTimerChanged += OnTimerChanged;
        WeightDropper.FirstWeightDropped += () => {started = true; OnTimerChanged(TrickTimer.instance.trickLength);};
        cycleStartTime = Time.time;
    }

    void OnDestroy()
    {
        TrickTimer.OnTimerChanged -= OnTimerChanged;
    }

    void OnTimerChanged(int seconds)
    {
        if (TrickTimer.instance && seconds == TrickTimer.instance.trickLength)
        {
            cycleStartTime = Time.time;
        }
    }

    void Update()
    {
        if (!radialImage || !TrickTimer.instance || !started)
        {
            return;
        }

        float elapsed = Time.time - cycleStartTime;
        radialImage.fillAmount = 1 - Mathf.Clamp01(elapsed/TrickTimer.instance.trickLength);
    }
}
