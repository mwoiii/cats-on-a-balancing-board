using Assets.CatsGame.Logic.Game;
using OMC.ECS;
using TMPro;
using UnityEngine;

namespace OMC.UI {
    public class HUDController : MonoBehaviour {

        public static HUDController instance;

        public GameObject gameOverScreen;
        public GameObject pauseScreen;

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
            CatCountBridgingSystem.CatCountChange += UpdateRemainingCats;
        }

        public void Start() {
            TrickTimer.OnLitterBonusChanged += OnLitterBonusChanged;
            TrickTimer.OnTimerChanged += OnLitterTimerChanged;

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

        private void UpdateRemainingCats(int difference) {
            UpdateRemainingCats(difference, true);
        }

        public void UpdateRemainingCats(int difference, bool doSpikeVibrate = true) {
            catCount += difference;
            counterText.text = catCount.ToString();
            if (doSpikeVibrate) {
                counterVibrate.IncrementSpike(Mathf.Abs(difference));
            }
        }

        public void InitiateGameOver() {
            int score = Mathf.Max((int)CatManagerScript.score, (int)GameLogicScript.score); // purely so both scenes can continue to work

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

        public void InitiatePause() {
            Time.timeScale = 0;
            if (pauseScreen) {
                pauseScreen.SetActive(true);
            }
        }

        public void TerminatePause() {
            Time.timeScale = GlobalTimescale.timeScale;
            if (pauseScreen) {
                pauseScreen.SetActive(false);
                foreach (var paw in pauseScreen.GetComponentsInChildren<RandomizedVibrate>()) // clear paws. the pause paws 
                {
                    Destroy(paw.gameObject);
                }
            }
        }
    }
}
