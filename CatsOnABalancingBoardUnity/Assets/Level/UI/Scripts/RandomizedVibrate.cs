using UnityEngine;

[RequireComponent(typeof(Vibrate))]
public class RandomizedVibrate : MonoBehaviour {

    public Vibrate vibrate;

    public float magMin = 0.1f;

    public float magMax = 10f;

    public float freqMin = 5f;

    public float freqMax = 100f;

    public void Awake() {
        if (!vibrate) {
            vibrate = GetComponent<Vibrate>();
        }

        if (!vibrate) {
            Debug.LogError("No vibrate on my randomized vibrate???");
            return;
        }

        vibrate.magnitude = Random.Range(magMin, magMax);
        vibrate.frequency = Random.Range(freqMin, freqMax);
    }
}
