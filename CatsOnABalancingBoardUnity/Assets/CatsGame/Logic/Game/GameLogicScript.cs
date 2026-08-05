using Assets.CatsGame.Logic.Game;
using UnityEngine;

public class GameLogicScript : MonoBehaviour {
    public static GameLogicScript instance;

    public static float gameTime = 0.0f;

    public static bool gameRunning = true;

    public static double score;

    public const double scoreScaleFactor = 0.05d;

    public int catCount { get; private set; }

    private void Update() {
        if (gameRunning) {
            gameTime += Time.deltaTime;
        }
    }

    void Start() {
        gameRunning = true;
        score = 0d;
    }

    void Awake() {
        instance = this;
        CatCountBridgingSystem.CatCountChange += CheckGameOver;
    }

    private void CheckGameOver(int difference) {
        catCount += difference;
        if (catCount <= 0 && gameRunning) {
            GameOver();
        }
    }

    public void GameOver() {
        gameRunning = false;
        HUDController.instance.InitiateGameOver();
        Debug.Log($"Game over at gameTime: {gameTime}");
    }
}
