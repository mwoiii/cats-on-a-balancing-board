using UnityEngine;
using UnityEngine.UI;

public class FadeOutAfterSeconds : MonoBehaviour {

    public float delay = 5f;

    public AnimationCurve fadeOutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    public bool disableOnFinish = true;

    public RawImage image;

    private float stopwatch;

    private bool startFade;

    public void Awake() {
        if (!image) {
            image = GetComponent<RawImage>();
        }
    }

    public void Update() {
        stopwatch += Time.deltaTime;
        if (!startFade && stopwatch > delay) {
            stopwatch = 0f;
            startFade = true;
        } else if (startFade && image) {
            Color newColor = image.color;
            newColor.a = fadeOutCurve.Evaluate(stopwatch);
            image.color = newColor;
            if (stopwatch >= fadeOutCurve.keys[^1].time && disableOnFinish) {
                gameObject.SetActive(false);
            }
        }
    }
}
