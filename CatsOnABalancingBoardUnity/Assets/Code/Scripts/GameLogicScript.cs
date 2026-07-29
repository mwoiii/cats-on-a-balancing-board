using UnityEngine;

public class GameLogicScript : MonoBehaviour {
    public static float gameTime = 0.0f;

    public static bool gameRunning = true;

    private void Update() {
        if (gameRunning) {
            gameTime += Time.deltaTime;
        }
    }

    void Start() {
        gameRunning = true;
    }

    public void GameOver() {
        gameRunning = false;
        HUDController.instance.InitiateGameOver();
        Debug.Log("Game Over");
        Debug.Log(gameTime);
    }
}
