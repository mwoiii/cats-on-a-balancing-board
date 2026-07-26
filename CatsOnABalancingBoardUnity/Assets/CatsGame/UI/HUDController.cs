using TMPro;
using UnityEngine;

public class HUDController : MonoBehaviour {

    public static HUDController instance;

    public GameObject gameOverScreen;

    public Vibrate counterVibrate;

    public TextMeshProUGUI counterText;

    public Vibrate litterVibrate;

    public TextMeshProUGUI litterText;

    public Vibrate litterTimerVibrate;

    public TextMeshProUGUI litterTimerText;

    private int catCount;

    public void Awake() {
        instance = this;
    }

    public void Start() {
        TrickTimer.onLitterBonusChanged += OnLitterBonusChanged;
        TrickTimer.onTimerChanged += OnLitterTimerChanged;
    }

    private void OnLitterTimerChanged(int time) {
        if (litterTimerText) {
            litterTimerText.text = time.ToString();
        }
    }

    private void OnLitterBonusChanged(int bonus) {
        if (litterText) {
            litterText.text = bonus.ToString();
        }
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
