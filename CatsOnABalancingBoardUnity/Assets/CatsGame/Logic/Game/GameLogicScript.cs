using System.Linq.Expressions;
using OMC.ECS;
using OMC.UI;
using Unity.Mathematics;
using UnityEngine;

namespace OMC {
    public class GameLogicScript : MonoBehaviour {
        public static GameLogicScript instance;

        public static float gameTime = 0.0f;

        public static bool gameRunning = true;
        public static bool firstWeightDropped = false;

        public static double score {get; private set;}

        public const double scoreScaleFactor = 0.05d;

        public int catCount { get; private set; }
        public long catCountAllTime { get; private set;}

        private void Update() {
            if (gameRunning) {
                gameTime += Time.deltaTime;
            }
        }

        void Start() {
            gameRunning = true;
            firstWeightDropped = false;
            score = 0d;
            WeightDropper.FirstWeightDropped += () => {firstWeightDropped = true;};
        }

        void Awake() {
            instance = this;
            CatCountBridgingSystem.CatCountChange += CheckGameOver;
        }

        private void CheckGameOver(int difference) {
            catCount += math.min(difference, int.MaxValue - catCount);
            catCountAllTime += math.max(difference, 0);
            if (catCount <= 0 && gameRunning) {
                GameOver();
            }
        }

        public void GameOver() {
            gameRunning = false;
            HUDController.instance.InitiateGameOver();
            Debug.Log($"Game over at gameTime: {gameTime}");
        }

        public void Pause() {
            gameRunning = false;
            HUDController.instance.InitiatePause();
            Debug.Log($"paused at gameTime: {gameTime}");
        }

        public void Unpause() {
            gameRunning = true;
            HUDController.instance.TerminatePause();
            Debug.Log($"unpaused at gameTime: {gameTime}");
        }

        public void AddToScore(double amount)
        {
            if (firstWeightDropped){score += amount;}
        }
    }
}
