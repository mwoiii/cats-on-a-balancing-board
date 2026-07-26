using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour {

    public static HUDController instance;

    public Vibrate counterVibrate;

    public TextMeshProUGUI counterText;

    public GameObject gameOverScreen;

    private int catCount;

    public void Awake() {
        instance = this;
    }

    public void UpdateRemainingCats(int difference, bool doSpikeVibrate = true) {
        catCount += difference;
        counterText.text = catCount.ToString();
        if (doSpikeVibrate) {
            counterVibrate.IncrementSpike(Mathf.Abs(difference));
        }
    }

    public void InitiateGameOver() {
        if (gameOverScreen) {
            gameOverScreen.SetActive(true);
        }
    }
}
