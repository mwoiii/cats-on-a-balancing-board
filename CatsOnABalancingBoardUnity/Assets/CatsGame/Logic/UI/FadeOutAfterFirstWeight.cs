using UnityEngine;
using UnityEngine.UI;

namespace OMC.UI {
    public class FadeOutAfterFirstWeight : MonoBehaviour {
        public AnimationCurve fadeOutCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        public bool disableOnFinish = true;

        public RawImage image;

        private float stopwatch;

        private bool startFade = false;

        public void Awake() {
            if (!image) {
                image = GetComponent<RawImage>();
            }
            WeightDropper.FirstWeightDropped += StartFade;
        }

        void StartFade() {
            startFade = true;
        }

        public void Update() {
            stopwatch += Time.deltaTime;
            if (!startFade) {
                stopwatch = 0f;
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
}
