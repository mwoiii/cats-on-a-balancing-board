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

    public Vibrate scoreVibrate;

    public TextMeshProUGUI scoreText;

    private int catCount;

    public void Awake() {
        instance = this;
    }

    public void Start() {
        TrickTimer.onLitterBonusChanged += OnLitterBonusChanged;
        TrickTimer.onTimerChanged += OnLitterTimerChanged;

        OnLitterBonusChanged(TrickTimer.litterBonus);
        OnLitterTimerChanged(TrickTimer.currentSecond);
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
        int score = (int)CatManagerScript.score;

        if (scoreText) {
            scoreText.text = score.ToString();
        }

        if (scoreVibrate) {
            scoreVibrate.magnitude = Mathf.Clamp(score * 0.005f, 0f, 25f);
        }

        if (gameOverScreen) {
            gameOverScreen.SetActive(true);
        }
    }
}
