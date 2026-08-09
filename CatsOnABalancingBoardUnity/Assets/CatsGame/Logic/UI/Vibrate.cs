using UnityEngine;

namespace OMC.UI {
    public class Vibrate : MonoBehaviour {

        private Vector3 localStartPos;

        public float magnitude;

        [SerializeField]
        private float _frequency = 1f;

        [SerializeField]
        private bool unscaledTime = false;

        public float frequency {
            get {
                return _frequency;
            }
            set {
                if (value == 0f) {
                    period = float.MaxValue;
                } else {
                    period = 1f / value;
                }
                _frequency = value;
            }
        }

        public float maxMagSpike = 500f;

        public float maxFreqSpike = 1000f;

        public float spikeIncFrac = 0.05f;

        public float spikeLerpSpeed = 5f;

        private float activePeriod;

        private float activeMagnitude;

        private float period;

        private float stopwatch;

        public void Awake() {
            if (frequency == 0f) {
                period = float.MaxValue;
            } else {
                period = 1f / frequency;
            }
        }

        public void Start() {
            activeMagnitude = magnitude;
            activePeriod = period;
            localStartPos = transform.localPosition;
        }

        public void Update() {
            activePeriod = Mathf.Lerp(activePeriod, period, Time.deltaTime * spikeLerpSpeed);
            activeMagnitude = Mathf.Lerp(activeMagnitude, magnitude, Time.deltaTime * spikeLerpSpeed);

            stopwatch += unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (stopwatch > activePeriod) {
                stopwatch = 0f;
                transform.localPosition = localStartPos + (new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f) * activeMagnitude);
            }
        }

        public void IncrementSpike(int count) {
            activePeriod = Mathf.Clamp(activePeriod - (1 - (1f / (maxFreqSpike * spikeIncFrac * count))), 1f / maxFreqSpike, Mathf.Infinity);
            activeMagnitude = Mathf.Clamp(activeMagnitude + maxMagSpike * spikeIncFrac * count, 0f, maxMagSpike);
        }
    }
}
